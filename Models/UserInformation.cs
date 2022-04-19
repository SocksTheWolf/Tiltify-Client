
using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class UserInformation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; protected set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; protected set; }
        [JsonProperty(PropertyName = "slug")]
        public string Slug { get; protected set; }
        [JsonProperty(PropertyName = "thumbnail")]
        public ThumbnailInformation Comment { get; protected set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; protected set; }
    }

    public class ThumbnailInformation
    {
        [JsonProperty(PropertyName = "src")]
        public string Src { get; protected set; }
        [JsonProperty(PropertyName = "alt")]
        public string Alt { get; protected set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; protected set; }
        [JsonProperty(PropertyName = "height")]
        public int Height { get; protected set; }
    }
}
