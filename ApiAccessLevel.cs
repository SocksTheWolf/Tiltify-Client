namespace Tiltify
{
    public enum ApiAccessLevel
    {
        None,
        OAuth,
        Public,
        Private,
    }

    public static class ApiAccessPath
    {
        public static string GetPath(ApiAccessLevel level, ApiVersion version)
        {
            // V3 doesn't add access level pathing
            if (version == ApiVersion.V3)
                return "";

            switch (level)
            {
                case ApiAccessLevel.OAuth:
                    return "/";
                case ApiAccessLevel.Public:
                    return "/public";
                case ApiAccessLevel.Private:
                    return "/private";
                case ApiAccessLevel.None:
                default:
                    return "/";
            }  
        }
    }
}
