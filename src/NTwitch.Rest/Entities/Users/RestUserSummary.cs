﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    public class RestUserSummary : RestEntity, IUser
    {
        [JsonProperty("logo")]
        public string LogoUrl { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }
        
        internal RestUserSummary(BaseRestClient client) : base(client) { }

        internal static RestUserSummary Create(BaseRestClient client, string json)
        {
            var user = new RestUserSummary(client);
            JsonConvert.PopulateObject(json, user);
            return user;
        }

        // Users
        public Task<RestBlockedUser> BlockAsync()
            => UserHelper.BlockAsync(this, Client, Id);
        public Task<RestChannelFollow> GetFollowAsync(uint channelId)
            => UserHelper.GetFollowAsync(this, Client, channelId);
        public Task<IEnumerable<RestChannelFollow>> GetFollowsAsync()
            => GetFollowsAsync(SortMode.CreatedAt);
        public Task<IEnumerable<RestChannelFollow>> GetFollowsAsync(SortMode mode = SortMode.CreatedAt, bool ascending = false, PageOptions options = null)
            => UserHelper.GetFollowsAsync(this, Client, mode, ascending, options);
        public Task UnblockAsync()
            => UserHelper.UnblockAsync(this, Client, Id);

        // IUser
        Task<IBlockedUser> IUser.BlockAsync() => null;
        Task IUser.UnblockAsync() => null;
        Task<IChannelFollow> IUser.GetFollowAsync(uint channelId) => null;
        Task<IEnumerable<IChannelFollow>> IUser.GetFollowsAsync() => null;
    }
}
