using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tiltify
{
    public class ApiBase
    {
        internal const string BaseTiltifyAPI = "https://tiltify.com/api";

        private readonly ApiSettings settings;
        private readonly IRateLimiter rateLimiter;
        private readonly IHttpCallHandler http;

        private readonly JsonSerializerSettings jsonDeserializer = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public ApiBase(ApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http)
        {
            this.settings = settings;
            this.rateLimiter = rateLimiter;
            this.http = http;
        }

        protected async Task<string> TiltifyGetAsync(string resource, ApiVersion api, List<KeyValuePair<string, string>> getParams = null, string customBase = null)
        {
            var url = ConstructResourceUrl(resource, getParams, api, customBase);
            var accessToken = settings.OAuthToken;

            return await rateLimiter.Perform(async () => (await http.GeneralRequestAsync(url, "GET", null, api, accessToken).ConfigureAwait(false)).Value).ConfigureAwait(false);
        }

        protected async Task<T> TiltifyGetGenericAsync<T>(string resource, ApiVersion api, List<KeyValuePair<string, string>> getParams = null, string customBase = null)
        {
            var url = ConstructResourceUrl(resource, getParams, api, customBase);
            var accessToken = settings.OAuthToken;

            return await rateLimiter.Perform(async () => JsonConvert.DeserializeObject<T>((await http.GeneralRequestAsync(url, "GET", null, api, accessToken).ConfigureAwait(false)).Value, jsonDeserializer)).ConfigureAwait(false);
        }

        private string ConstructResourceUrl(string resource = null, List<KeyValuePair<string, string>> getParams = null, ApiVersion api = ApiVersion.V3, string overrideUrl = null)
        {
            var url = "";
            if (overrideUrl == null)
            {
                if (resource == null)
                    throw new Exception("Cannot pass null resource with null override url");
                switch (api)
                {
                    case ApiVersion.V3:
                        url = $"{BaseTiltifyAPI}/v3{resource}";
                        break;
                }
            }
            else
            {
                url = resource == null ? overrideUrl : $"{overrideUrl}{resource}";
            }
            if (getParams != null)
            {
                for (var i = 0; i < getParams.Count; i++)
                {
                    if (i == 0)
                        url += $"?{getParams[i].Key}={Uri.EscapeDataString(getParams[i].Value)}";
                    else
                        url += $"&{getParams[i].Key}={Uri.EscapeDataString(getParams[i].Value)}";
                }
            }
            return url;
        }
    }
}
