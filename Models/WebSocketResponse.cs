using Newtonsoft.Json.Linq;

namespace Tiltify.Models
{
    // https://hexdocs.pm/phoenix/Phoenix.Socket.Message.html
    public class WebSocketResponse
    {
        public string JoinId { get; }
        public string MessageId { get; }
        public string Topic { get; }
        public string Event { get; }
        public JToken Data { get; }

        public WebSocketResponse(string json)
        {
            var array = JArray.Parse(json);

            JoinId = array[0]?.ToString();
            MessageId = array[1].ToString();
            Topic = array[2].ToString();
            Event = array[3].ToString();
            Data = array[4];
        }
    }
}
