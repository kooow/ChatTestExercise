using System;

namespace SignalRChatTest.Models
{
    /// <summary>
    /// Customer conversation message entity.
    /// </summary>
    public class CustomerConversationMessage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerConversationId">Customer conversation Id</param>
        /// <param name="content">context</param>
        public CustomerConversationMessage(int customerConversationId, string content)
        {
            CustomerConversationId = customerConversationId;
            Content = String.Copy(content);
            Created = DateTime.Now;
        }

        /// <summary>
        /// Identifier of the message
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Customer conversation Id this message belongs to.
        /// </summary>
        public int CustomerConversationId { get; set; }

        /// <summary>
        /// Connection Id of the customer conversation this message belongs to.
        /// </summary>
        public CustomerConversation CustomerConversation { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Created date of the message.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Is the message from customer?
        /// </summary>
        public bool FromCustomer { get; set; }
    }
}
