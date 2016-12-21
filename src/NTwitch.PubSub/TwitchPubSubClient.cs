﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.PubSub
{
    public partial class TwitchPubSubClient : ITwitchClient
    {
        private PubSubSocketClient SocketClient { get; }
        public string SocketUrl { get; }

        public TwitchPubSubClient() : this(new TwitchPubSubConfig()) { }
        public TwitchPubSubClient(TwitchPubSubConfig config)
        {
            SocketUrl = config.SocketUrl;
        }
        
        // ITwitchClient
        public ConnectionState ConnectionState { get; } = ConnectionState.Disconnected;

        public Task LoginAsync(string clientid, string token = null)
        {
            throw new NotImplementedException();
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }
    }
}
