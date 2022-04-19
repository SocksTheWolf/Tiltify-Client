using System;
using Tiltify.Models;

namespace Tiltify.Events
{
    /// <inheritdoc />
    /// <summary>
    /// Class representing arguments for a listen response.
    /// </summary>
    public class OnListenResponseArgs : EventArgs
    {
        /// <summary>
        /// Property representing the topic that was listened to
        /// </summary>
        public string Topic;
        /// <summary>
        /// Property representing the response as Response object
        /// </summary>
        public WebSocketResponse Response;
        /// <summary>
        /// Property representing if request was successful.
        /// </summary>
        public bool Successful;
        /// <summary>
        /// Property representing the id of the channel the event originated from.
        /// </summary>
        public string ChannelId;
    }
}
