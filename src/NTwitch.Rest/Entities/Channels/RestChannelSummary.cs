﻿using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NTwitch.Pubsub")]
namespace NTwitch.Rest
{
    public class RestChannelSummary : IEntity, IChannelSummary
    {
        public TwitchRestClient Client { get; }
        [JsonProperty("_id")]
        public ulong Id { get; internal set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }

        internal RestChannelSummary(ITwitchClient client)
        {
            Client = client as TwitchRestClient;
        }

        ITwitchClient IEntity.Client
            => Client;
    }
}
