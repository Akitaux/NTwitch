﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.PubSub
{
    public class TwitchPubSubConfig
    {
        public string SocketUrl { get; set; } = "wss://pubsub-edge.twitch.tv";
    }
}
