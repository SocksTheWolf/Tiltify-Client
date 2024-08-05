
using Newtonsoft.Json;

namespace Tiltify.Models
{
    public class Money
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency = string.Empty;

        [JsonProperty(PropertyName = "value")]
        public string Value = string.Empty;
    }

    public class DonationInformation
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; protected set; }
#nullable enable
        [JsonProperty(PropertyName = "campaign_id")]
        public string? CampaignId { get; protected set; }

        [JsonProperty(PropertyName = "poll_option_id")]
        public string? PollOptionId { get; protected set; }

        [JsonProperty(PropertyName = "amount")]
        public Money? Amount { get; protected set; }
#nullable disable
        [JsonProperty(PropertyName = "donor_name")]
        public string Name { get; protected set; }

        [JsonProperty(PropertyName = "donor_comment")]
        public string Comment { get; protected set; }

        [JsonProperty(PropertyName = "completed_at")]
        public string CompletedAt { get; protected set; }
    }
}
