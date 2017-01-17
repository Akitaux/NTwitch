﻿using NTwitch.Rest;
using System;

namespace NTwitch.Chat
{
    public class MessageBase
    {
        public BaseRestClient Client { get; }
        public DateTime UtcTimestamp { get; } = DateTime.UtcNow;

        internal MessageBase(BaseRestClient client)
        {
            Client = client;
        }
    }
}
