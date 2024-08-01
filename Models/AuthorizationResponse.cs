using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class AuthorizationResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken;

        [JsonProperty(PropertyName = "created_at")]
        public string CreationTime;

        [JsonProperty(PropertyName = "expires_in")]
        public int Expiration;

        [JsonProperty(PropertyName = "refresh_token")]
        public string? RefreshToken;

        [JsonProperty(PropertyName = "scope")]
        public string Scope;

        [JsonProperty(PropertyName = "token_type")]
        public string Type;
    }
}
