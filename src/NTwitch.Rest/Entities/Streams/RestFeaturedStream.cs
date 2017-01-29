﻿using Newtonsoft.Json;

namespace NTwitch.Rest
{
    public class RestFeaturedStream : RestEntity
    {
        [JsonProperty("image")]
        public string ImageUrl { get; private set; }
        [JsonProperty("priority")]
        public int Priority { get; private set; }
        [JsonProperty("scheduled")]
        public bool IsScheduled { get; private set; }
        [JsonProperty("sponsored")]
        public bool IsSponsored { get; private set; }
        [JsonProperty("stream")]
        public RestStream Stream { get; private set; }
        [JsonProperty("text")]
        public string Description { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }

        public RestFeaturedStream(BaseRestClient client) : base(client) { }

        public static RestFeaturedStream Create(BaseRestClient client, string json)
        {
            var stream = new RestFeaturedStream(client);
            JsonConvert.PopulateObject(json, stream);
            return stream;
        }
    }
}
