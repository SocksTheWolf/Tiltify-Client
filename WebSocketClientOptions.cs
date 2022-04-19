using System;
using Tiltify.Models;

namespace Tiltify
{
    public class WebSocketClientOptions
    {
        public int SendQueueCapacity { get; set; } = 10000;
        public TimeSpan SendCacheItemTimeout { get; set; } = TimeSpan.FromMinutes(30);
        public ushort SendDelay { get; set; } = 50;
        public ReconnectionPolicy ReconnectionPolicy { get; set; } = new ReconnectionPolicy(3000, maxAttempts: 10);
        public int DisconnectWait { get; set; } = 20000;
        public TimeSpan ThrottlingPeriod { get; set; } = TimeSpan.FromSeconds(30);
        public int MessagesAllowedInPeriod { get; set; } = 100;
    }
}