using Newtonsoft.Json;

namespace Tiltify.Models
{
#nullable enable
    public class TeamCampaignInfo
    {
        [JsonProperty(PropertyName = "amount_raised")]
        public Money? TeamAmountRaised { get; set; }

        [JsonProperty(PropertyName = "currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "donate_url")]
        public string DonateURL { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "goal")]
        public Money? Goal { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "supporting_amount_raised")]
        public Money? SupportingAmountsRaised { get; set; }

        [JsonProperty(PropertyName = "total_amount_raised")]
        public Money? TotalAmountRaised { get; set; }
    }
#nullable restore
}
