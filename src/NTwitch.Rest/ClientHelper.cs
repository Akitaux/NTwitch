﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    internal static class ClientHelper
    {
        //
        ////  Issue #7
        ////  Properties don't fill as intended.
        //
        public static async Task<RestStream> GetStreamAsync(BaseRestClient client, ulong id, StreamType? type)
        {
            var request = new RequestOptions();
            if (type != null)
                request.Parameters.Add("stream_type", type.ToString().ToLower());

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TwitchConverter(client, "stream"));
            
            string json = await client.ApiClient.SendAsync("GET", $"streams/{id}", request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RestStream>(json, settings);
        }

        public static async Task<RestSelfUser> GetCurrentUserAsync(BaseRestClient client)
        {
            string json = await client.ApiClient.SendAsync("GET", "user").ConfigureAwait(false);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TwitchConverter(client));

            return JsonConvert.DeserializeObject<RestSelfUser>(json, settings);
        }

        public static async Task<RestSelfChannel> GetCurrentChannelAsync(BaseRestClient client)
        {
            var json = await client.ApiClient.SendAsync("GET", "channel").ConfigureAwait(false);
            return RestSelfChannel.Create(client, json);
        }

        public static async Task<RestChannel> GetChannelAsync(BaseRestClient client, ulong id)
        {
            var json = await client.ApiClient.SendAsync("GET", $"channels/{id}").ConfigureAwait(false);
            return RestChannel.Create(client, json);
        }

        public static async Task<IEnumerable<RestTopGame>> GetTopGamesAsync(BaseRestClient client, PageOptions options)
        {
            var request = new RequestOptions();
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "games/top", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("top"));
            return items.Select(x => RestTopGame.Create(client, x));
        }

        //
        ////  Issue #8
        ////  Properties don't fill as intended.
        //
        public static async Task<IEnumerable<RestIngest>> GetIngestsAsync(BaseRestClient client)
        {
            string json = await client.ApiClient.SendAsync("GET", "ingests");
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("ingests"));
            return items.Select(x => JsonConvert.DeserializeObject<RestIngest>(json));
        }

        public static async Task<IEnumerable<RestGame>> FindGamesAsync(BaseRestClient client, string query, bool? islive)
        {
            var request = new RequestOptions();
            request.Parameters.Add("query", query);
            if (islive != null)
                request.Parameters.Add("live", islive);

            string json = await client.ApiClient.SendAsync("GET", "search/games", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("games"));
            return items.Select(x => RestGame.Create(client, x));
        }

        public static async Task<IEnumerable<RestChannel>> FindChannelsAsync(BaseRestClient client, string query, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("query", query);
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "search/channels", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("channels"));
            return items.Select(x => RestChannel.Create(client, x));
        }

        public static async Task<IEnumerable<RestStream>> FindStreamsAsync(BaseRestClient client, string query, bool? hls, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("query", query);
            if (hls != null)
                request.Parameters.Add("hls", hls);
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "search/streams", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("streams"));
            return items.Select(x => RestStream.Create(client, x));
        }

        public static async Task<IEnumerable<RestVideo>> GetTopVideosAsync(BaseRestClient client, string game, VideoPeriod? period, BroadcastType? type, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("game", game);
            if (period != null)
                request.Parameters.Add("period", period.ToString().ToLower());
            if (type != null)
                request.Parameters.Add("broadcast_type", type.ToString().ToLower());
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "videos/top", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("vods"));
            return items.Select(x => RestVideo.Create(client, x));
        }

        public static async Task<RestUser> FindUserAsync(BaseRestClient client, string name)
        {
            var request = new RequestOptions();
            request.Parameters.Add("login", name);

            string json = await client.ApiClient.SendAsync("GET", "users", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("users"));
            return RestUser.Create(client, items.FirstOrDefault());
        }

        //
        ////  Issue #9
        ////  Properties don't fill as intended.
        //
        public static async Task<IEnumerable<RestStream>> GetStreamsAsync(BaseRestClient client, string game, uint[] channelids, string language, StreamType? type, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("game", game);
            if (channelids != null)
                request.Parameters.Add("channel", string.Join(",", channelids));
            if (language != null)
                request.Parameters.Add("language", language);
            if (type != null)
            request.Parameters.Add("type", type.ToString().ToLower());
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "streams", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("streams"));
            return items.Select(x => RestStream.Create(client, x));
        }

        //
        ////  Issue #10
        ////  Needs jsonconverter logic for passing client to custom constructor.
        //
        public static async Task<IEnumerable<RestFeaturedStream>> GetFeaturedStreamsAsync(BaseRestClient client, PageOptions options)
        {
            var request = new RequestOptions();
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "streams/featured", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("featured"));
            return items.Select(x => RestFeaturedStream.Create(client, x));
        }

        public static async Task<IEnumerable<RestTeamSummary>> GetTeamsAsync(BaseRestClient client, PageOptions options)
        {
            var request = new RequestOptions();
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "teams", request);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(json, new TwitchCollectionConverter("teams"));
            return items.Select(x => RestTeamSummary.Create(client, x));
        }

        public static async Task<RestUser> GetUserAsync(BaseRestClient client, ulong id)
        {
            string json = await client.ApiClient.SendAsync("GET", $"users/{id}");
            return RestUser.Create(client, json);
        }

        //
        ////  Issue #10
        ////  Needs jsonconverter logic for passing client to custom constructor.
        //
        public static async Task<RestTeam> GetTeamAsync(BaseRestClient client, string name)
        {
            string json = await client.ApiClient.SendAsync("GET", $"teams/{name}");
            return RestTeam.Create(client, json);
        }

        public static async Task<RestStreamSummary> GetStreamSummaryAsync(BaseRestClient client, string game)
        {
            var request = new RequestOptions();
            request.Parameters.Add("game", game);

            string json = await client.ApiClient.SendAsync("GET", "streams/summary", request);
            return JsonConvert.DeserializeObject<RestStreamSummary>(json);
        }
    }
}
