using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SignalRChatTest.Data.Repositories;
using SignalRChatTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRChatTest.Controllers
{
    /// <summary>
    /// Controller for managing customer conversations.
    /// </summary>
    public class CustomerConversationsController : Controller
    {
        private readonly CustomerConversationRepository m_repository;

        /// <summary>
        /// Constructor for CustomerConversationsController, used to inject the repository dependency.
        /// </summary>
        /// <param name="repository">Repository</param>
        public CustomerConversationsController(CustomerConversationRepository repository)
        {
            m_repository = repository;
        }

        /// <summary>
        /// Returns a view with a list of customer conversations.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            var conversations = m_repository.GetCustomerConversations();
            return View(conversations);
        }

        /// <summary>
        /// Returns a messages for a specific conversation identified by the connection ID.
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <returns>Messages by connection Id</returns>
        [HttpGet]
        [Authorize]
        [EnableCors("AllowSpecificOrigin")]
        public ActionResult GetConversationMessages(string connectionId)
        {
            var messages = m_repository.GetMessagesByConnectionId(connectionId);

            var messageList = messages.Select(m => new
            {
                content = m.Content,
                created = $"{m.Created.ToShortDateString()} {m.Created.ToShortTimeString()}",
                fromcustomer = m.FromCustomer
            });
            return Json(messageList);
        }

        /// <summary>
        /// Returns a list of modified customer conversations since the last refresh time.
        /// </summary>
        /// <returns>Modified customer conversations</returns>
        [HttpGet]
        [Authorize]
        [EnableCors("AllowSpecificOrigin")]
        public ActionResult<List<CustomerConversation>> GetModified()
        {
            DateTime nowMinus3Seconds = DateTime.Now.AddSeconds(-3);
            var modifiedConversations = m_repository.GetModifiedCustomerConversations(nowMinus3Seconds);

            var list = modifiedConversations.Select(mc => new
            {
                connectionid = mc.ConnectionId,
                lastmodified = $"{mc.LastModified.ToShortDateString()} {mc.LastModified.ToShortTimeString()}",
                created = $"{mc.Created.ToShortDateString()} {mc.Created.ToShortTimeString()}",
                unanswered = mc.Unanswered,
                closed = mc.Closed
            });

            return Json(list);
        }
    }
}