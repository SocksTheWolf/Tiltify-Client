using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class GetCampaignDonationsResponse
    {
        [JsonProperty(PropertyName = "data")]
        public DonationInformation[] Data { get; protected set; }
    }
}