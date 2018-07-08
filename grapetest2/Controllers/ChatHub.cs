using grapetest2.Data;
using grapetest2.Data.Repositories;
using grapetest2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace grapetest2.Controllers
{

  /// <summary>
  /// 
  /// </summary>
  public class ChatHub : Hub
  {

    private readonly CustomerConversationRepository repository;

    private readonly IHttpContextAccessor httpContext;

    /// <summary>
    ///
    /// </summary>
    static volatile public string activeOperatorConnectionId;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="repository"></param>
    public ChatHub(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, CustomerConversationRepository repository)
    {
      this.httpContext = httpContextAccessor;
      this.repository = repository;
    }

    [Authorize]
    public async Task MessageFromOperator(string connectionId, string message)
    {

      CustomerConversation conversation = this.repository.GetCustomerConversationByConnectionId(connectionId);
      if (conversation != null)
      {
        CustomerConversationMessage ccMessage = new CustomerConversationMessage(conversation.Id, message);
        ccMessage.FromCustomer = false;
        this.repository.AddMessage(ccMessage);

        conversation.Unanswered = false;
        conversation.LastModified = DateTime.Now;
        this.repository.Update(conversation);

        var messageJsonResult = new JsonResult(GetMessageJsonObject(ccMessage));

        await Clients.Client(connectionId).SendAsync("ReceiveMessage", connectionId, messageJsonResult.Value);
      }
    }

    [AllowAnonymous]
    public override async Task OnConnectedAsync()
    {

      if (this.httpContext.HttpContext.User.Identity.IsAuthenticated)
      {
        if (!String.Equals(activeOperatorConnectionId, this.Context.ConnectionId))
          activeOperatorConnectionId = this.Context.ConnectionId;          
      }
      else
      {
        CustomerConversation conversation = this.repository.GetCustomerConversationByConnectionId(this.Context.ConnectionId);
        if (conversation != null)
        {
          conversation.LastModified = DateTime.Now;
          conversation.Closed = false;
          this.repository.Update(conversation);
        }
        else
        {
          CustomerConversation customerConversation = new CustomerConversation(this.Context.ConnectionId);
          customerConversation = this.repository.Add(customerConversation);
        }
      }
    }

    [AllowAnonymous]
    public override async Task OnDisconnectedAsync(Exception exception)
    {

      // operator disconnected
      if (this.httpContext.HttpContext.User.Identity.IsAuthenticated) activeOperatorConnectionId = null;  
      else 
      {
        // close the conversation
        CustomerConversation searchedConversation = this.repository.GetCustomerConversationByConnectionId(this.Context.ConnectionId);

        searchedConversation.LastModified = DateTime.Now;
        searchedConversation.Closed = true;
        this.repository.Update(searchedConversation);
      }
    }

    [AllowAnonymous]
    public async Task MessageFromCustomer(string message)
    {
      CustomerConversation searchedConversation =
          this.repository.GetCustomerConversationByConnectionId(this.Context.ConnectionId);

      if (searchedConversation != null)
      {
        searchedConversation.Unanswered = true;
        searchedConversation.LastModified = DateTime.Now;
        this.repository.Update(searchedConversation);
      }
      else
      {
        searchedConversation = new CustomerConversation(this.Context.ConnectionId);
        searchedConversation = this.repository.Add(searchedConversation);
      }

      CustomerConversationMessage ccMessage = new CustomerConversationMessage(searchedConversation.Id, message);
      ccMessage.FromCustomer = true;
      this.repository.AddMessage(ccMessage);

      // if we have active operator
      if (activeOperatorConnectionId != null)
      {
        var messageJsonResult = new JsonResult(GetMessageJsonObject(ccMessage));
         
        await Clients.Client(activeOperatorConnectionId).SendAsync("ReceiveMessage", this.Context.ConnectionId, messageJsonResult.Value);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ccMessage"></param>
    /// <returns></returns>
    private dynamic GetMessageJsonObject(CustomerConversationMessage ccMessage)
    {
      dynamic messageJson = new ExpandoObject();
      messageJson.content = ccMessage.Content;
      messageJson.created = ccMessage.Created.ToShortDateString() + " " + ccMessage.Created.ToShortTimeString();
      messageJson.fromcustomer = ccMessage.FromCustomer;
      return messageJson;
    }

  }
}
