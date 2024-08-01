namespace Tiltify
{
    public class ApiSettings
    {
        public string ClientID { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string OAuthToken { get; set; }

        public ApiVersion APIVersion {get; set;} = ApiVersion.Latest;
    }
}
