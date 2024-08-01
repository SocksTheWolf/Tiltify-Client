using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tiltify
{
    public class ApiBase
    {
        protected ApiSettings settings;
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

        protected async Task<T> TiltifyPostGenericAsync<T>(string resource, ApiVersion api, List<KeyValuePair<string, string>> getParams = null, string customBase = null, ApiAccessLevel access = ApiAccessLevel.Public)
        {
            var url = ConstructResourceUrl(resource, null, api, customBase, access);
            string payload = "";

            if (getParams != null)
            {
                Dictionary<string, string> converted = new Dictionary<string, string>();
                // Fun conversion from a KVP to a Dictionary so it can convert properly via JSONConvert
                getParams.ForEach((singleKvp) => converted.Add(singleKvp.Key, singleKvp.Value));
                payload = JsonConvert.SerializeObject(converted);
            }

            string accessToken = string.Empty;
            if (access != ApiAccessLevel.OAuth)
                accessToken = settings.OAuthToken;

            return await rateLimiter.Perform(async () => JsonConvert.DeserializeObject<T>((await http.GeneralRequestAsync(url, "POST", payload, api, accessToken).ConfigureAwait(false)).Value, jsonDeserializer)).ConfigureAwait(false);
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
                        url = $"https://v5api.tiltify.com{accessPath}{resource}";
                        break;
                }
            }
            else
            {
                url = resource == null ? overrideUrl : $"{overrideUrl}{resource}";
            }

            if (getParams != null)
            {
                // Check to see if we have a ? and if not, we'll add the ?
                if (!url.Contains('?'))
                {
                    url += "?";
                }

                for (var i = 0; i < getParams.Count; i++)
                {
                    url += $"&{getParams[i].Key}={Uri.EscapeDataString(getParams[i].Value)}";
                }
            }
            return url;
        }
    }
}
