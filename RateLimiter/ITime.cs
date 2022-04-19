using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tiltify.RateLimiter
{
    public interface ITime
    {
        DateTime GetTimeNow();

        Task GetDelay(TimeSpan timespan, CancellationToken cancellationToken);
    }
}
