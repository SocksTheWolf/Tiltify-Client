using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tiltify
{
    public class ApiBase
    {
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

        public ApiVersion GetApiVersion() => settings.APIVersion;

        protected async Task<string> TiltifyGetAsync(string resource, ApiVersion api, List<KeyValuePair<string, string>> getParams = null, string customBase = null, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            var url = ConstructResourceUrl(resource, getParams, api, customBase, access);
            var accessToken = settings.OAuthToken;

            return await rateLimiter.Perform(async () => (await http.GeneralRequestAsync(url, "GET", null, api, accessToken).ConfigureAwait(false)).Value).ConfigureAwait(false);
        }

        protected async Task<T> TiltifyGetGenericAsync<T>(string resource, ApiVersion api, List<KeyValuePair<string, string>> getParams = null, string customBase = null, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            var url = ConstructResourceUrl(resource, getParams, api, customBase, access);
            var accessToken = settings.OAuthToken;

            return await rateLimiter.Perform(async () => JsonConvert.DeserializeObject<T>((await http.GeneralRequestAsync(url, "GET", null, api, accessToken).ConfigureAwait(false)).Value, jsonDeserializer)).ConfigureAwait(false);
        }

        private string ConstructResourceUrl(string resource = null, List<KeyValuePair<string, string>> getParams = null, ApiVersion api = ApiVersion.Latest, string overrideUrl = null, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            var url = "";
            if (overrideUrl == null)
            {
                if (resource == null)
                    throw new Exception("Cannot pass null resource with null override url");

                string accessPath = ApiAccessPath.GetPath(access, api);
                switch (api)
                {
                    case ApiVersion.V3:
                        url = $"https://tiltify.com/api/v3{resource}";
                        break;
                    case ApiVersion.V5:
                        url = $"https://v5api.tiltify.com/api{accessPath}{resource}";
                        break;
                }
            }
            else
            {
                url = resource == null ? overrideUrl : $"{overrideUrl}{resource}";
            }

            // Check to see if we have a ? and if not, we'll add the ?
            if (!url.Contains('?'))
            {
                url += "?";
            }

            if (getParams != null)
            {
                for (var i = 0; i < getParams.Count; i++)
                {
                    url += $"&{getParams[i].Key}={Uri.EscapeDataString(getParams[i].Value)}";
                }
            }
            return url;
        }
    }
}
