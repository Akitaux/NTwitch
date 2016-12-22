﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.WebSocket
{
    public class SocketSelfChannel : SocketChannel, ISelfChannel
    {
        public string Email { get; }
        public string StreamKey { get; }

        internal SocketSelfChannel(TwitchSocketClient client, ulong id) : base(client, id) { }

        public Task GetEditorsAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetSubscriberAsync(uint id)
        {
            throw new NotImplementedException();
        }

        public Task GetSubscribersAsync(bool descending = true, TwitchPageOptions page = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<ModifyChannelParams> action)
        {
            throw new NotImplementedException();
        }

        public Task ResetStreamKeyAsync()
        {
            throw new NotImplementedException();
        }

        public Task StartCommercialAsync()
        {
            throw new NotImplementedException();
        }
    }
}
