﻿using Newtonsoft.Json;
using System;

namespace NTwitch.Rest
{
    public class RestFollow : FollowBase
    {
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; internal set; }

        internal RestFollow(BaseRestClient client) : base(client) { }

        internal static RestFollow Create(BaseRestClient client, string json)
        {
            var follow = new RestFollow(client);
            JsonConvert.PopulateObject(json, follow);
            return follow;
        }
    }
}
