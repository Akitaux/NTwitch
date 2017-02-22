﻿using Newtonsoft.Json;
using NTwitch.Rest;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NTwitch.Pubsub
{
    public partial class TwitchPubsubClient : BaseRestClient, ITwitchClient
    {
        private SocketClient _socket;
        private ConcurrentDictionary<string, Func<PubsubMessage, Task>> _subscriptions;
        private string _token = null;
        private string _host;
        
        public TwitchPubsubClient() : base(new TwitchPubsubConfig()) { }
        public TwitchPubsubClient(TwitchPubsubConfig config) : base(config)
        {
            _host = config.PubsubUrl;
            _subscriptions = new ConcurrentDictionary<string, Func<PubsubMessage, Task>>();
        }
        
        public async Task LoginAsync(TokenType type, string token)
        {
            _token = token;
            await LoginInternalAsync(type, _token);
        }

        public async Task ConnectAsync()
        {
            _socket = new SocketClient(_host, _token);
            _socket.EventReceived += OnEventReceivedAsync;
            await _socket.ConnectAsync();
        }
        
        public Task DisconnectAsync()
            => _socket.DisconnectAsync();

        private Task OnEventReceivedAsync(PubsubMessage msg)
        {
            if (_subscriptions.TryGetValue(msg.Data.Topic, out Func<PubsubMessage, Task> action))
                action.Invoke(msg);

            return Task.CompletedTask;
        }

        public async Task SubscribeAsync(PubsubTopic topic, Func<PubsubMessage, Task> action)
        {
            await _socket.SendAsync("LISTEN", topic.ToString());

            if (!_subscriptions.TryAdd(topic.ToString(), action))
                throw new Exception("Unable to add listen.");
        }

        public async Task UnsubscribeAsync(PubsubTopic topic)
        {
            await _socket.SendAsync("UNLISTEN", topic.ToString());

            if (!_subscriptions.TryRemove(topic.ToString(), out Func<PubsubMessage, Task> value))
                throw new Exception("Unable to remove listen.");
        }
    }
}
