using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRChatTest.Data;
using SignalRChatTest.Data.Repositories;
using SignalRChatTest.Models;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace SignalRChatTest.Controllers
{
    /// <summary>
    /// ChatHub is a SignalR hub that handles real-time communication between customers and operators.
    /// </summary>
    public class ChatHub : Hub
    {
        private const string ReceiveMessageMethodName = "ReceiveMessage";

        private readonly CustomerConversationRepository m_repository;
        private readonly IHttpContextAccessor m_httpContext;

        /// <summary>
        /// The connection ID of the currently active operator. This is used to send messages to the operator when a customer sends a message.
        /// </summary>
        static volatile public string activeOperatorConnectionId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        /// <param name="repository">Customer conversation repository</param>
        public ChatHub(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, CustomerConversationRepository repository)
        {
            m_httpContext = httpContextAccessor;
            m_repository = repository;
        }

        /// <summary>
        /// Message from operator to customer.
        /// </summary>
        /// <param name="connectionId">Current connection id</param>
        /// <param name="message">current message</param>
        /// <returns></returns>
        [Authorize]
        public async Task MessageFromOperator(string connectionId, string message)
        {
            var conversation = m_repository.GetCustomerConversationByConnectionId(connectionId);
            if (conversation != null)
            {
                var customerConversationMessage = new CustomerConversationMessage(conversation.Id, message)
                {
                    FromCustomer = false
                };
                m_repository.AddMessage(customerConversationMessage);

                conversation.Unanswered = false;
                conversation.LastModified = DateTime.Now;

                m_repository.Update(conversation);

                var messageJsonResult = new JsonResult(GetMessageJsonObject(customerConversationMessage));

                await Clients.Client(connectionId).SendAsync(ReceiveMessageMethodName, connectionId, messageJsonResult.Value);
            }
        }

        /// <summary>
        /// Connect handler for the SignalR hub. This method is called when a client connects to the hub.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task OnConnectedAsync()
        {
            if (m_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!String.Equals(activeOperatorConnectionId, Context.ConnectionId))
                {
                    activeOperatorConnectionId = Context.ConnectionId;
                }
            }
            else
            {
                var conversation = m_repository.GetCustomerConversationByConnectionId(Context.ConnectionId);
                if (conversation != null)
                {
                    conversation.LastModified = DateTime.Now;
                    conversation.Closed = false;
                    m_repository.Update(conversation);
                }
                else
                {
                    var customerConversation = new CustomerConversation(Context.ConnectionId);
                    _ = m_repository.Add(customerConversation);
                }
            }
        }

        /// <summary>
        /// Disconnect handler for the SignalR hub. This method is called when a client disconnects from the hub.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // operator disconnected
            if (m_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                activeOperatorConnectionId = null;
            }
            else
            {
                // close the conversation
                var searchedConversation = m_repository.GetCustomerConversationByConnectionId(Context.ConnectionId);

                searchedConversation.LastModified = DateTime.Now;
                searchedConversation.Closed = true;

                m_repository.Update(searchedConversation);
            }
        }

        /// <summary>
        /// Message from customer to operator.
        /// </summary>
        /// <param name="message">Current message</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task MessageFromCustomer(string message)
        {
            var searchedConversation = m_repository.GetCustomerConversationByConnectionId(Context.ConnectionId);

            if (searchedConversation != null)
            {
                searchedConversation.Unanswered = true;
                searchedConversation.LastModified = DateTime.Now;
                m_repository.Update(searchedConversation);
            }
            else
            {
                searchedConversation = new CustomerConversation(Context.ConnectionId);
                searchedConversation = m_repository.Add(searchedConversation);
            }

            var customerConversationMessage = new CustomerConversationMessage(searchedConversation.Id, message)
            {
                FromCustomer = true
            };
            m_repository.AddMessage(customerConversationMessage);

            // if we have active operator
            if (activeOperatorConnectionId != null)
            {
                var messageJsonResult = new JsonResult(GetMessageJsonObject(customerConversationMessage));

                await Clients.Client(activeOperatorConnectionId).SendAsync(ReceiveMessageMethodName, Context.ConnectionId, messageJsonResult.Value);
            }
        }

        /// <summary>
        /// Returns a JSON object representing the customer conversation message.
        /// </summary>
        /// <param name="message">Customer conversation message</param>
        /// <returns>Json object</returns>
        private dynamic GetMessageJsonObject(CustomerConversationMessage message)
        {
            dynamic messageJson = new ExpandoObject();
            messageJson.content = message.Content;
            messageJson.created = $"{message.Created.ToShortDateString()} {message.Created.ToShortTimeString()}";
            messageJson.fromcustomer = message.FromCustomer;
            return messageJson;
        }
    }
}
