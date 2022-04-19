using System.Threading.Tasks;
using Tiltify.Models;

namespace Tiltify
{
    public class Users : ApiBase
    {
        public Users(ApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        public Task<GetUserResponse> GetUser()
        {
            return TiltifyGetGenericAsync<GetUserResponse>($"/user", ApiVersion.V3);
        }
    }
}
