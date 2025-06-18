using System;

namespace SignalRChatTest.Models
{
    /// <summary>
    /// Entity representing a conversation with a customer.
    /// </summary>
    public class CustomerConversation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionId">connection Id</param>
        public CustomerConversation(string connectionId)
        {
            this.ConnectionId = String.Copy(connectionId);
            this.Created = DateTime.Now;
            this.LastModified = DateTime.Now;
            this.Unanswered = true;
            this.Closed = false;
        }

        /// <summary>
        /// Identifier of the conversation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Connection Id of the customer.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Is the conversation unanswered?
        /// </summary>
        public bool Unanswered { get; set; }

        /// <summary>
        /// Created date of the conversation.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Last modified date of the conversation.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Is the conversation closed?
        /// </summary>
        public bool Closed { get; set; }
    }
}
