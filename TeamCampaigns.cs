using System.Collections.Generic;
using System.Threading.Tasks;
using Tiltify.Models;
using System;
using System.Globalization;

namespace Tiltify
{
    public class TeamCampaigns : ApiBase
    {
        public TeamCampaigns(ApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        public Task<GetTeamCampaignResponse> GetCampaign(string campaignId)
        {
            return TiltifyGetGenericAsync<GetTeamCampaignResponse>($"/team_campaigns/{campaignId}/", GetApiVersion(), null, null, ApiAccessLevel.Public);
        }

        public Task<GetCampaignDonationsResponse> GetCampaignDonations(string campaignId, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            return TiltifyGetGenericAsync<GetCampaignDonationsResponse>($"/team_campaigns/{campaignId}/donations", GetApiVersion(), null, null, access);
        }

        public Task<GetCampaignDonationsResponse> GetCampaignDonations(string campaignId, DateTime donationsAfter, int limit = 10, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            List<KeyValuePair<string, string>> Args = new List<KeyValuePair<string, string>>();
            Args.Add(new KeyValuePair<string, string>("completed_after", donationsAfter.ToString("o", CultureInfo.InvariantCulture)));
            return TiltifyGetGenericAsync<GetCampaignDonationsResponse>($"/team_campaigns/{campaignId}/donations?limit={limit}", GetApiVersion(), Args, null, access);
        }
    }
}
