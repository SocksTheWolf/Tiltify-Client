using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class GetTeamCampaignResponse
    {
        [JsonProperty(PropertyName = "data")]
        public TeamCampaignInfo Data { get; protected set; }
    }
}
