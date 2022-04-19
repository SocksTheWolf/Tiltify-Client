namespace Tiltify.Models
{
    /// <summary>
    /// Model representing the previous request.
    /// </summary>
    class PreviousRequest
    {
        /// <summary>
        /// Unique communication token.
        /// </summary>
        /// <value>The nonce.</value>
        public string MessageId { get; }
        /// <summary>
        /// Topic that we are interested in.
        /// </summary>
        /// <value>The topic.</value>
        public string Topic { get; }

        /// <summary>
        /// PreviousRequest model constructor.
        /// </summary>
        /// <param name="messageId">The nonce.</param>
        /// <param name="topic">The topic.</param>
        public PreviousRequest(string messageId, string topic = "none set")
        {
            MessageId = messageId;
            Topic = topic;
        }
    }
}
