using System;
using System.Collections.Generic;
using System.Linq;
using grapetest2.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using grapetest2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace grapetest2.Controllers
{

  /// <summary>
  /// 
  /// </summary>
  public class CustomerConversationsController : Controller
  {
    /// <summary>
    /// 
    /// </summary>
    private readonly CustomerConversationRepository repository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public CustomerConversationsController(CustomerConversationRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public IActionResult Index()
    {    
      List<CustomerConversation> conversations = this.repository.GetCustomerConversations();
      return View(conversations);
    }

    [HttpGet]
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    public ActionResult GetConversationMessages(string connectionId)
    {
      List<CustomerConversationMessage> messages = this.repository.GetMessagesByConnectionId(connectionId);

      var list = messages
      .Select(m => new {
        content = m.Content,
        created = m.Created.ToShortDateString() + " " + m.Created.ToShortTimeString(),
        fromcustomer = m.FromCustomer
      });

      return Json(list);
    }

    [HttpGet]
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    public ActionResult<List<CustomerConversation>> GetModified()
    {
      DateTime nowMinus3Seconds = DateTime.Now.AddSeconds(-3);
      List<CustomerConversation> modifiedConversations = this.repository.GetModifiedCustomerConversations(nowMinus3Seconds);

      var list = modifiedConversations
          .Select(a => new
          {
            connectionid = a.ConnectionId,
            lastmodified = a.LastModified.ToShortDateString() + " " + a.LastModified.ToShortTimeString(),
            created = a.Created.ToShortDateString() + " " + a.Created.ToShortTimeString(),
            unanswered = a.Unanswered,
            closed = a.Closed
          });

      return Json(list);
    }

  }
}