using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Tiltify.Events;
using Tiltify.Models;
using Timer = System.Timers.Timer;

namespace Tiltify
{
    public class TiltifyWebSocket
    {
        /// <summary>
        /// The socket
        /// </summary>
        private readonly WebSocketClient _socket;
        public bool IsConnected => _socket.IsConnected;
        /// <summary>
        /// The previous requests
        /// </summary>
        private readonly List<PreviousRequest> _previousRequests = new List<PreviousRequest>();
        /// <summary>
        /// The previous requests semaphore
        /// </summary>
        private readonly Semaphore _previousRequestsSemaphore = new Semaphore(1, 1);
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<TiltifyWebSocket> _logger;
        /// <summary>
        /// The ping timer
        /// </summary>
        private readonly Timer _pingTimer = new Timer();
        /// <summary>
        /// The pong timer
        /// </summary>
        private readonly Timer _pongTimer = new Timer();
        /// <summary>
        /// The pong received
        /// </summary>
        private bool _pongReceived = false;
        /// <summary>
        /// The topic list
        /// </summary>
        private readonly List<string> _topicList = new List<string>();

        private readonly Dictionary<string, string> _topicToChannelId = new Dictionary<string, string>();

        private int websocketMessageId = 16;

        #region Events
        /// <summary>
        /// Fires when this client receives any data from Tiltify
        /// </summary>
        public event EventHandler<OnLogArgs> OnLog;
        /// <inheritdoc />
        /// <summary>
        /// Fires when Tiltify Service is connected.
        /// </summary>
        public event EventHandler OnTiltifyServiceConnected;
        /// <inheritdoc />
        /// <summary>
        /// Fires when Tiltify Service is closed.
        /// </summary>
        public event EventHandler OnTiltifyServiceClosed;
        /// <summary>
        /// Occurs when [on pub sub service error].
        /// </summary>
        event EventHandler<OnTiltifyServiceErrorArgs> OnTiltifyServiceError;
        /// <inheritdoc />
        /// <summary>
        /// Fires when Tiltify receives any response.
        /// </summary>
        public event EventHandler<OnListenResponseArgs> OnListenResponse;
        public event EventHandler<OnCampaignDonationArgs> OnCampaignDonation;
        #endregion

        /// <summary>
        /// Constructor for a client that interface's with Tiltify's websocket.
        /// </summary>
        public TiltifyWebSocket(ILogger<TiltifyWebSocket> logger = null, WebSocketClientOptions options = null)
        {
            _logger = logger;

            _socket = new WebSocketClient(options);

            _socket.OnConnected += Socket_OnConnected;
            _socket.OnError += OnError;
            _socket.OnMessage += OnMessage;
            _socket.OnDisconnected += Socket_OnDisconnected;

            _pongTimer.Interval = 10000;
            _pongTimer.Elapsed += PongTimerTick;
        }

        /// <summary>
        /// Handles the <see cref="E:OnError" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="OnErrorEventArgs"/> instance containing the event data.</param>
        private void OnError(object sender, OnErrorEventArgs e)
        {
            _logger?.LogError($"OnError in Tiltify Websocket connection occured! Exception: {e.Exception}");
            OnTiltifyServiceError?.Invoke(this, new OnTiltifyServiceErrorArgs { Exception = e.Exception });
        }

        /// <summary>
        /// Handles the <see cref="E:OnMessage" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="OnMessageEventArgs"/> instance containing the event data.</param>
        private void OnMessage(object sender, OnMessageEventArgs e)
        {
            _logger?.LogDebug($"Received Websocket OnMessage: {e.Message}");
            OnLog?.Invoke(this, new OnLogArgs { Data = e.Message });
            ParseMessage(e.Message);
        }

        /// <summary>
        /// Handles the OnDisconnected event of the Socket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Socket_OnDisconnected(object sender, EventArgs e)
        {
            _logger?.LogWarning("Tiltify Websocket connection closed");
            _pingTimer.Stop();
            _pongTimer.Stop();
            OnTiltifyServiceClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Handles the OnConnected event of the Socket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Socket_OnConnected(object sender, EventArgs e)
        {
            _logger?.LogInformation("Tiltify Websocket connection established");
            _pingTimer.Interval = 30000;
            _pingTimer.Elapsed += PingTimerTick;
            _pingTimer.Start();
            OnTiltifyServiceConnected?.Invoke(this, null);
        }

        /// <summary>
        /// Pings the timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void PingTimerTick(object sender, ElapsedEventArgs e)
        {
            //Reset pong state.
            _pongReceived = false;

            var messageId = GenerateMessageId();

            //Send ping.
            var data = new JArray(
                null,
                messageId,
                "phoenix",
                "heartbeat",
                new JObject()
            );
            _socket.Send(data.ToString());

            //Start pong timer.
            _pongTimer.Start();
        }

        /// <summary>
        /// Pongs the timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void PongTimerTick(object sender, ElapsedEventArgs e)
        {
            //Stop the pong timer.
            _pongTimer.Stop();

            if (_pongReceived)
            {
                //If we received a pong we're good.
                _pongReceived = false;
            }
            else
            {
                //Otherwise we're disconnected so close the socket.
                _socket.Close();
            }
        }

        /*
        * Heartbeat required every 30 seconds
        * JOIN: ["27","27","campaign.165613.donation","phx_join",{}]
        * JOIN REPLY: ["27","27","campaign.165613.donation","phx_reply",{"response":{},"status":"ok"}]
        * HEARTBEAT: [null,"32","phoenix","heartbeat",{}]
        * HEARTBEAT REPLY: [null,"32","phoenix","phx_reply",{"response":{},"status":"ok"}]
        * LEAVE: ["27","28","campaign.165613.donation","phx_leave",{}]
        * LEAVE REPLY: ["27","28","campaign.165613.donation","phx_reply",{"response":{},"status":"ok"}]
        * CLOSE REPLY: ["27","27","campaign.165613.donation","phx_close",{}]
        * DONATION: [null,null,"campaign.165613.donation","donation",{"amount":37.0,"challenge_id":31290,"comment":"Muensterous cheese puns! I may not be that sharp, but this is no gouda! Swiss entire idea is full of holes! Cheddar get out of here before I make a pun myself...\n((And of course, as ever and always, Trans Rights!!!))","completedAt":1649552307000,"event_id":165613,"id":5827276,"name":"N0nb1naryCode","poll_option_id":39030,"reward_id":null,"updatedAt":1649552307000}]
        * DONATION: [null,null,"campaign.165613.donation","donation",{"amount":12.0,"challenge_id":31290,"comment":"Let's gooooooo! Animals in balls? Running crazy races? We need this!","completedAt":1649553051000,"event_id":165613,"id":5827305,"name":"TrainerAnade","poll_option_id":39031,"reward_id":null,"updatedAt":1649553051000}]
        * DONATION: [null,null,"campaign.165613.donation","donation",{"amount":21.0,"challenge_id":31290,"comment":"Things and stuff! Trans Rights! I don't have anything clever to add!!","completedAt":1649553226000,"event_id":165613,"id":5827315,"name":"N0nb1naryCode","poll_option_id":39030,"reward_id":null,"updatedAt":1649553226000}]
        */
        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ParseMessage(string message)
        {
            var resp = new WebSocketResponse(message);

            if (resp.Topic == null)
            {
                return;
            }
            if (int.TryParse(resp.JoinId, out int joinId)) {
                int highestId = Math.Max(websocketMessageId, joinId);
                Interlocked.Exchange(ref websocketMessageId, highestId);
            }
            var topicParts = resp.Topic.Split('.');

            switch (resp.Event?.ToLower())
            {
                case "phx_close":
                    _socket.Close();
                    return;
                case "phx_reply":
                    if ("phoenix".Equals(resp.Topic))
                    {
                        // Heartbeat
                        _pongReceived = true;
                        return;
                    }
                    if (_previousRequests.Count != 0)
                    {
                        bool handled = false;
                        _previousRequestsSemaphore.WaitOne();
                        try
                        {
                            for (int i = 0; i < _previousRequests.Count;)
                            {
                                var request = _previousRequests[i];
                                if (string.Equals(request.MessageId, resp.MessageId, StringComparison.CurrentCulture))
                                {
                                    //Remove the request.
                                    _previousRequests.RemoveAt(i);
                                    _topicToChannelId.TryGetValue(request.Topic, out var requestChannelId);
                                    OnListenResponse?.Invoke(this, new OnListenResponseArgs { Response = resp, Topic = request.Topic, Successful = true, ChannelId = requestChannelId });
                                    handled = true;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                        }
                        finally
                        {
                            _previousRequestsSemaphore.Release();
                        }
                        if (handled) return;
                    }
                    break;
                case "donation":
                    if (topicParts.Length < 3)
                    {
                        break;
                    }
                    if ("campaign".Equals(topicParts[0]) && "donation".Equals(topicParts[2]))
                    {
                        var donation = resp.Data.ToObject<DonationInformation>();
                        OnCampaignDonation?.Invoke(this, new OnCampaignDonationArgs { Donation = donation });
                        return;
                    }
                    break;
                case "reconnect": _socket.Close(); break;
            }
            UnaccountedFor(message);
        }

        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();
        /// <summary>
        /// Generates the nonce.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GenerateMessageId()
        {
            return Interlocked.Increment(ref websocketMessageId).ToString();
        }

        /// <summary>
        /// Listens to topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        private void ListenToTopic(string topic)
        {
            _topicList.Add(topic);
        }

        /// <summary>
        /// Listen to multiple topics.
        /// </summary>
        /// <param name="topics">The topics</param>
        private void ListenToTopics(params string[] topics)
        {
            foreach (var topic in topics)
            {
                _topicList.Add(topic);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Sends the topics.
        /// </summary>
        /// <param name="unlisten">if set to <c>true</c> [unlisten].</param>
        public void SendTopics(bool unlisten = false)
        {
            var messageId = GenerateMessageId();

            var topics = new List<string>();
            _previousRequestsSemaphore.WaitOne();
            try
            {
                foreach (var val in _topicList)
                {
                    _previousRequests.Add(new PreviousRequest(messageId, val));
                    topics.Add(val);
                }
            }
            finally
            {
                _previousRequestsSemaphore.Release();
            }

            foreach (var topic in topics)
            {
                var jsonData = new JArray(
                    messageId,
                    messageId,
                    topic,
                    !unlisten ? "phx_join" : "phx_leave",
                    new JObject()
                    );

                _socket.Send(jsonData.ToString());
            }

            _topicList.Clear();
        }

        /// <summary>
        /// Unaccounted for.
        /// </summary>
        /// <param name="message">The message.</param>
        private void UnaccountedFor(string message)
        {
            _logger?.LogInformation($"[Tiltify] [Unaccounted] {message}");
        }

        #region Listeners
        /// <inheritdoc />
        /// <summary>
        /// Sends a request to listenOn follows coming into a specified channel.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        public void ListenToCampaignDonations(string campaignId)
        {
            // campaign.165613.donation
            var topic = $"campaign.{campaignId}.donation";
            _topicToChannelId[topic] = campaignId;
            ListenToTopic(topic);
        }
        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Method to connect to Tiltify's service. You MUST listen toOnConnected event and listen to a Topic within 15 seconds of connecting (or be disconnected)
        /// </summary>
        public void Connect()
        {
            _socket.Open();
        }

        /// <inheritdoc />
        /// <summary>
        /// What do you think it does? :)
        /// </summary>
        public void Disconnect()
        {
            _socket.Close();
        }

    }
}
