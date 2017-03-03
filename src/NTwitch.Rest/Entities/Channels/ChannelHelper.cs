﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    internal static class ChannelHelper
    {
        public static async Task<IEnumerable<RestPost>> GetPostsAsync(IChannel channel, BaseRestClient client, int comments, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("comments", comments);
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", $"feed/{channel.Id}/posts", request);
            return JsonConvert.DeserializeObject<IEnumerable<RestPost>>(json, new TwitchCollectionConverter(client, "posts"));
        }
        
        public static async Task<RestClip> GetClipAsync(IChannel channel, BaseRestClient client, string id)
        {
            var json = await client.ApiClient.SendAsync("GET", $"clips/{channel.Name}/{id}");
            return JsonConvert.DeserializeObject<RestClip>(json, new TwitchEntityConverter(client));
        }
        
        public static async Task<IEnumerable<RestClip>> GetTopClipsAsync(IChannel channel, BaseRestClient client, string game, VideoPeriod? period, bool? istrending, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("channel", channel.Name);
            request.Parameters.Add("game", game);
            request.Parameters.Add("period", Enum.GetName(typeof(VideoPeriod), period).ToLower());
            if (istrending != null)
                request.Parameters.Add("trending", istrending);
            if (options != null)
            {
                request.Parameters.Add("limit", options?.Limit);
                request.Parameters.Add("offset", options?.Offset);
            }

            string json = await client.ApiClient.SendAsync("GET", "clips/top", request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<RestClip>>(json, new TwitchCollectionConverter(client, "clips"));
        }
        
        public static async Task<IEnumerable<RestClip>> GetFollowedClipsAsync(IChannel channel, BaseRestClient client, bool istrending = false, int limit = 10)
        {
            var request = new RequestOptions();
            request.Parameters.Add("trending", istrending);
            request.Parameters.Add("limit", limit);

            string json = await client.ApiClient.SendAsync("GET", "clips/followed", request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<RestClip>>(json, new TwitchCollectionConverter(client, "clips"));
        }

        public static Task UnfollowAsync(IChannel channel, BaseRestClient client)
        {
            throw new NotImplementedException();
        }

        public static Task FollowAsync(IChannel channel, BaseRestClient client, bool notify)
        {
            throw new NotImplementedException();
        }

        public static async Task<RestPost> GetPostAsync(IChannel channel, BaseRestClient client, ulong id, int comments)
        {
            var request = new RequestOptions();
            request.Parameters.Add("comments", comments);

            string json = await client.ApiClient.SendAsync("GET", "feed/" + channel.Id + "/posts/" + id, request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RestPost>(json, new TwitchEntityConverter(client));
        }
        
        public static async Task<IEnumerable<RestUserFollow>> GetFollowersAsync(IChannel channel, BaseRestClient client, bool ascending, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("direction", ascending ? "asc" : "desc");
            request.Parameters.Add("limit", options?.Limit);
            request.Parameters.Add("offset", options?.Offset);

            string json = await client.ApiClient.SendAsync("GET", $"channels/{channel.Id}/followers", request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<RestUserFollow>>(json, new TwitchCollectionConverter(client, "follows"));
        }
        
        public static Task<IEnumerable<RestTeam>> GetTeamsAsync(IChannel channel, BaseRestClient client)
        {
            throw new NotImplementedException();
        }
        
        public static Task<IEnumerable<RestVideo>> GetVideosAsync(IChannel channel, BaseRestClient client, string language, SortMode sort, BroadcastType type, PageOptions options)
        {
            throw new NotImplementedException();
        }
        
        //
        // SelfChannel
        //

        public static Task<RestPost> CreatePostAsync(ISelfChannel channel, BaseRestClient client, Action<CreatePostParams> args)
        {
            throw new NotImplementedException();
        }

        public static Task<RestPost> DeletePostAsync(ISelfChannel channel, BaseRestClient client, uint postid)
        {
            throw new NotImplementedException();
        }

        public static Task<IEnumerable<RestUser>> GetEditorsAsync(ISelfChannel channel, BaseRestClient client)
        {
            throw new NotImplementedException();
        }

        public static async Task<IEnumerable<RestUserSubscription>> GetSubscribersAsync(ISelfChannel channel, BaseRestClient client, bool ascending, PageOptions options)
        {
            var request = new RequestOptions();
            request.Parameters.Add("direction", ascending ? "asc" : "desc");
            request.Parameters.Add("limit", options?.Limit);
            request.Parameters.Add("offset", options?.Offset);

            string json = await client.ApiClient.SendAsync("GET", "channels/" + channel.Id + "/subscriptions", request).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<RestUserSubscription>>(json, new TwitchCollectionConverter(client, "subscriptions"));
        }

        public static async Task<RestUserSubscription> GetSubscriberAsync(ISelfChannel channel, BaseRestClient client, uint userid)
        {
            string json = await client.ApiClient.SendAsync("GET", "channels/" + channel.Id + "/subscriptions/" + userid).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RestUserSubscription>(json, new TwitchEntityConverter(client));
        }

        public static Task StartCommercialAsync(ISelfChannel channel, BaseRestClient client, int duration)
        {
            throw new NotImplementedException();
        }

        public static Task<RestSelfChannel> ResetStreamKeyAsync(ISelfChannel channel, BaseRestClient client)
        {
            throw new NotImplementedException();
        }

        public static Task<RestSelfChannel> ModifyAsync(ISelfChannel channel, BaseRestClient client, Action<ModifyChannelParams> args)
        {
            throw new NotImplementedException();
        }
    }
}
