using System.Collections.Generic;
using System.Threading.Tasks;
using Tiltify.Models;

namespace Tiltify
{
    public class Auth : ApiBase
    {
        public enum AuthGrantType
        {
            None,
            ClientCredentials,
            AuthorizationCode,
            RefreshToken
        }

        public string GrantTypeToString(AuthGrantType grantType)
        {
            switch(grantType)
            {
                default:
                case AuthGrantType.None:
                    return "";
                case AuthGrantType.ClientCredentials:
                    return "client_credentials";
                case AuthGrantType.AuthorizationCode:
                    return "authorization_code";
                case AuthGrantType.RefreshToken:
                    return "refresh_token";
            }
        }

        public Auth(ApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        public Task<AuthorizationResponse> Authorize(AuthGrantType grantType = AuthGrantType.ClientCredentials, ApiAccessLevel scopes = ApiAccessLevel.Public, string authCode = "")
        {
            List<KeyValuePair<string, string>> Args = new List<KeyValuePair<string, string>>();
            Args.Add(new KeyValuePair<string, string>("grant_type", GrantTypeToString(grantType)));
            Args.Add(new KeyValuePair<string, string>("client_id", settings.ClientID));
            Args.Add(new KeyValuePair<string, string>("client_secret", settings.ClientSecret));

            if (grantType == AuthGrantType.AuthorizationCode)
            {
                Args.Add(new KeyValuePair<string, string>("code", authCode));
            }
            else if (grantType == AuthGrantType.RefreshToken) 
            {
                Args.Add(new KeyValuePair<string, string>("refresh_token", settings.RefreshToken));
            }

            // Adding the scopes
            string scopeForm;
            switch(scopes)
            {
                default:
                case ApiAccessLevel.Public:
                    scopeForm = "public";
                    break;
                case ApiAccessLevel.Private:
                    scopeForm = "webhooks:write";
                    break;
                case ApiAccessLevel.PublicPrivate:
                    scopeForm = "public webhooks:write";
                    break;
            }
            Args.Add(new KeyValuePair<string, string>("scope", scopeForm));

            Task<AuthorizationResponse> authResp = TiltifyPostGenericAsync<AuthorizationResponse>("/oauth/token", GetApiVersion(), Args, null, ApiAccessLevel.OAuth);
            if (authResp.Result != null)
            {
                AuthorizationResponse resp = authResp.Result;
                if (resp.Type == "bearer")
                    settings.OAuthToken = resp.AccessToken;

                if (resp.RefreshToken != null)
                    settings.RefreshToken = resp.RefreshToken;
            }
            return authResp;
        }
    }
}
