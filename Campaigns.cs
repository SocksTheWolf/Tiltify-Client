using System.Threading.Tasks;
using Tiltify.Models;

namespace Tiltify
{
    public class Campaigns : ApiBase
    {
        public Campaigns(ApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        public Task<GetCampaignDonationsResponse> GetCampaignDonations(int campaignId, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            return TiltifyGetGenericAsync<GetCampaignDonationsResponse>($"/campaigns/{campaignId}/donations", GetApiVersion(), null, null, access);
        }
    }
}
