using Microsoft.Extensions.Logging;
using Tiltify.RateLimiter;

namespace Tiltify
{
    public class Tiltify
    {
        private readonly ILogger<Tiltify> logger;
        public ApiSettings Settings { get; }

        public Auth Auth { get; }
        public Campaigns Campaigns { get; }
        public Users Users { get; }

        /// <summary>
        /// Creates an Instance of the Tiltify Class.
        /// </summary>
        /// <param name="loggerFactory">Instance of LoggerFactory, otherwise no logging is used, </param>
        /// <param name="rateLimiter">Instance of RateLimiter, otherwise no ratelimiter is used.</param>
        /// <param name="settings">Instance of ApiSettings, otherwise defaults used, can be changed later</param>
        /// <param name="http">Instance of HttpCallHandler, otherwise default handler used</param>
        public Tiltify(ILoggerFactory loggerFactory = null, IRateLimiter rateLimiter = null, ApiSettings settings = null, IHttpCallHandler http = null)
        {
            logger = loggerFactory?.CreateLogger<Tiltify>();
            rateLimiter = rateLimiter ?? BypassLimiter.CreateLimiterBypassInstance();
            http = http ?? new TiltifyhHttpClient(loggerFactory?.CreateLogger<TiltifyhHttpClient>());
            Settings = settings ?? new ApiSettings();

            Auth = new Auth(settings, rateLimiter, http);
            Campaigns = new Campaigns(settings, rateLimiter, http);
            Users = new Users(settings, rateLimiter, http);
        }
    }
}
