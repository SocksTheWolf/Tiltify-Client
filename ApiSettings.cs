namespace Tiltify
{
    public class ApiSettings
    {
        public string OAuthToken { get; set; }

        public ApiVersion APIVersion {get; set;} = ApiVersion.Latest;
    }
}
