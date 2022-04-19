using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class GetUserResponse
    {
        [JsonProperty(PropertyName = "data")]
        public UserInformation Data { get; protected set; }
    }
}