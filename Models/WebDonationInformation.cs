
using Newtonsoft.Json;

namespace Tiltify.Models
{
    //{ "amount":1.0,"challenge_id":null,"comment":null,"completedAt":1650338036000,"event_id":165720,"id":5842193,"name":"Anonymous","poll_option_id":null,"reward_id":null,"updatedAt":1650338036000}
    public class WebDonationInformation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; protected set; }
        [JsonProperty(PropertyName = "event_id")]
        public int? CampaignId { get; protected set; }
        [JsonProperty(PropertyName = "challenge_id")]
        public int? ChallengeId { get; protected set; }
        [JsonProperty(PropertyName = "poll_option_id")]
        public int? PollOptionId { get; protected set; }
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; protected set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; protected set; }
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; protected set; }
        [JsonProperty(PropertyName = "completedAt")]
        public long CompletedAt { get; protected set; }
        [JsonProperty(PropertyName = "reward_id")]
        public int? RewardId { get; protected set; }
        [JsonProperty(PropertyName = "rewardId")]
        private int? RewardIdAltKey { set { RewardId = value; } }
    }
}
