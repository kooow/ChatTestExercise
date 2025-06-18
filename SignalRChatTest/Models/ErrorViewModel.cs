namespace SignalRChatTest.Models
{
    /// <summary>
    /// ViewModel for error pages.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Request identifier for the current request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Is the request identifier available?
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}