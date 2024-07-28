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
            ApiVersion version = GetApiVersion();
            string endpointPath;
            switch (version)
            {
                default:
                case ApiVersion.V3:
                    endpointPath = "/user";
                    break;
                case ApiVersion.V5:
                    endpointPath = "/current-user";
                    break;
            }
            return TiltifyGetGenericAsync<GetUserResponse>(endpointPath, version);
        }
    }
}
