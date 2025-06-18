using Microsoft.EntityFrameworkCore.ChangeTracking;
using SignalRChatTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRChatTest.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerConversationRepository
    {
        protected readonly ApplicationDbContext m_context;

        /// <summary>
        /// Construcitor for CustomerConversationRepository, used to inject the database context dependency.
        /// </summary>
        /// <param name="context">Context</param>
        public CustomerConversationRepository(ApplicationDbContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Returns a list of customer conversations ordered by last modified date in descending order.
        /// </summary>
        /// <returns>list of customer conversations</returns>
        public List<CustomerConversation> GetCustomerConversations()
        {
            return m_context.CustomerConversations.OrderByDescending(cc => cc.LastModified).ToList();
        }

        /// <summary>
        /// Add a new customer conversation to the database.
        /// </summary>
        /// <param name="customerConversation">customer conversation</param>
        /// <returns>Added customer conversation</returns>
        public CustomerConversation Add(CustomerConversation customerConversation)
        {
            EntityEntry<CustomerConversation> newCustomerConversation = m_context.CustomerConversations.Add(customerConversation);

            m_context.SaveChanges();
            return newCustomerConversation.Entity;
        }

        /// <summary>
        /// Adds a new message to an existing customer conversation.
        /// </summary>
        /// <param name="message">Customer conversation message</param>
        /// <returns>Added new message entity</returns>
        public CustomerConversationMessage AddMessage(CustomerConversationMessage message)
        {
            EntityEntry<CustomerConversationMessage> newMessage = m_context.CustomerConversationMessages.Add(message);
            m_context.SaveChanges();
            return newMessage.Entity;
        }

        /// <summary>
        /// Updates an existing customer conversation in the database.
        /// </summary>
        /// <param name="customerConversation">Customer conversation</param>
        public void Update(CustomerConversation customerConversation)
        {
            m_context.CustomerConversations.Update(customerConversation);
            m_context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing customer conversation message in the database.
        /// </summary>
        /// <param name="message">Existing customer conversation</param>
        public void UpdateMessage(CustomerConversationMessage message)
        {
            m_context.CustomerConversationMessages.Update(message);
            m_context.SaveChanges();
        }

        /// <summary>
        /// Returns a customer conversation by its connection ID.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns>Customer conversation entity</returns>
        public CustomerConversation GetCustomerConversationByConnectionId(string connectionId)
        {
            return m_context.CustomerConversations.FirstOrDefault(cc => cc.ConnectionId == connectionId);
        }

        /// <summary>
        /// Returns a list of customer conversations that have been modified after the specified date and time.
        /// </summary>
        /// <param name="lastResfreshDateTime">Last datetime</param>
        /// <returns>List of customer conversation</returns>
        public List<CustomerConversation> GetModifiedCustomerConversations(DateTime lastResfreshDateTime)
        {
            List<CustomerConversation> list = m_context.CustomerConversations
                .Where(cc => cc.LastModified > lastResfreshDateTime)
                .OrderBy(cc => cc.LastModified).ToList();

            return list;
        }

        /// <summary>
        /// Returns a list of messages for a specific customer conversation identified by the connection ID.
        /// </summary>
        /// <param name="connectionId">Connection id</param>
        /// <returns>List of messages</returns>
        public List<CustomerConversationMessage> GetMessagesByConnectionId(string connectionId)
        {
            List<CustomerConversationMessage> messages =
                 (from ccm in m_context.CustomerConversationMessages
                  from cc in m_context.CustomerConversations
                  where cc.ConnectionId == connectionId
                  where ccm.CustomerConversationId == cc.Id
                  orderby ccm.Created ascending
                  select ccm).ToList();
            return messages;
        }
    }
}
