using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tiltify.Events;

namespace Tiltify
{
    class Throttlers
    {
        public readonly BlockingCollection<Tuple<DateTime, string>> SendQueue =
            new BlockingCollection<Tuple<DateTime, string>>();

        public bool Reconnecting { get; set; } = false;
        public bool ShouldDispose { get; set; } = false;
        public CancellationTokenSource TokenSource { get; set; }
        public bool ResetThrottlerRunning;
        public int SentCount = 0;
        public Task ResetThrottler;

        private readonly TimeSpan _throttlingPeriod;
        private readonly WebSocketClient _client;

        public Throttlers(WebSocketClient client, TimeSpan throttlingPeriod)
        {
            _throttlingPeriod = throttlingPeriod;
            _client = client;
        }

        public void StartThrottlingWindowReset()
        {
            ResetThrottler = Task.Run(async () =>
            {
                ResetThrottlerRunning = true;
                while (!ShouldDispose && !Reconnecting)
                {
                    Interlocked.Exchange(ref SentCount, 0);
                    await Task.Delay(_throttlingPeriod, TokenSource.Token);
                }

                ResetThrottlerRunning = false;
                return Task.CompletedTask;
            });
        }

        public void IncrementSentCount()
        {
            Interlocked.Increment(ref SentCount);
        }

        public Task StartSenderTask()
        {
            StartThrottlingWindowReset();

            return Task.Run(async () =>
            {
                try
                {
                    while (!ShouldDispose)
                    {
                        await Task.Delay(_client.Options.SendDelay);

                        if (SentCount == _client.Options.MessagesAllowedInPeriod)
                        {
                            _client.MessageThrottled(new OnMessageThrottledEventArgs
                            {
                                Message =
                                    "Message Throttle Occured. Too Many Messages within the period specified in WebsocketClientOptions.",
                                AllowedInPeriod = _client.Options.MessagesAllowedInPeriod,
                                Period = _client.Options.ThrottlingPeriod,
                                SentMessageCount = Interlocked.CompareExchange(ref SentCount, 0, 0)
                            });

                            continue;
                        }

                        if (!_client.IsConnected || ShouldDispose) continue;

                        var msg = SendQueue.Take(TokenSource.Token);
                        if (msg.Item1.Add(_client.Options.SendCacheItemTimeout) < DateTime.UtcNow) continue;
                        
                        try
                        {
                            await _client.SendAsync(Encoding.UTF8.GetBytes(msg.Item2));

                            IncrementSentCount();
                        }
                        catch (Exception ex)
                        {
                            _client.SendFailed(new OnSendFailedEventArgs { Data = msg.Item2, Exception = ex });
                            break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // This is okay, we are shutting down
                }
                catch (Exception ex)
                {
                    _client.SendFailed(new OnSendFailedEventArgs { Data = "", Exception = ex });
                    _client.Error(new OnErrorEventArgs { Exception = ex });
                }
            });
        }

    }
}
