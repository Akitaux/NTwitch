﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    internal static class ClientHelper
    {
        #region Users

        public static async Task<RestSelfUser> GetCurrentUserAsync(BaseRestClient client)
        {
            if (!client.Token.IsValid)
                throw new NotSupportedException("You must log in with oauth to get the current user.");
            if (!client.Token.Authorization.Scopes.Contains("user_read"))
                throw new MissingScopeException("user_read");

            var model = await client.RestClient.GetCurrentUserAsync();
            var entity = new RestSelfUser(client, model.Id);
            entity.Update(model);
            return entity;
        }

        public static async Task<RestUser> GetUserAsync(BaseRestClient client, ulong id)
        {
            var model = await client.RestClient.GetUserAsync(id);
            var entity = new RestUser(client, model.Id);
            entity.Update(model);
            return entity;
        }
        
        public static async Task<IEnumerable<RestUser>> GetUsersAsync(BaseRestClient client, string[] usernames)
        {
            var model = await client.RestClient.GetUsersAsync(usernames);
            var entity = model.Users.Select(x =>
            {
                var user = new RestUser(client, x.Id);
                user.Update(x);
                return user;
            });
            return entity;
        }

        #endregion
        #region Channels

        public static async Task<RestSelfChannel> GetCurrentChannelAsync(BaseRestClient client)
        {
            if (!client.Token.IsValid)
                throw new NotSupportedException("You must log in with oauth to get the current channel.");
            if (!client.Token.Authorization.Scopes.Contains("channel_read"))
                throw new MissingScopeException("channel_read");

            var model = await client.RestClient.GetCurrentChannelAsync();
            var entity = new RestSelfChannel(client, model.Id);
            entity.Update(model);
            return entity;
        }

        public static async Task<RestChannel> GetChannelAsync(BaseRestClient client, ulong channelId)
        {
            var model = await client.RestClient.GetChannelAsync(channelId);
            var entity = new RestChannel(client, model.Id);
            entity.Update(model);
            return entity;
        }

        public static async Task<IEnumerable<RestCheerInfo>> GetCheersAsync(BaseRestClient client, ulong? channelId)
        {
            var model = await client.RestClient.GetCheersAsync(channelId);
            var entity = model.Actions.Select(x => new RestCheerInfo(client, x));
            return entity;
        }

        #endregion
        #region Communities

        public static async Task<RestCommunity> GetCommunityAsync(BaseRestClient client, string id, bool isname = false)
        {
            var model = await client.RestClient.GetCommunityAsync(id, isname);
            var entity = new RestCommunity(client, model.Id);
            entity.Update(model);
            return entity;
        }

        public static Task ModifyChannelAsync(Action<ModifyCommunityParams> properties)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Videos

        public static async Task<RestVideo> GetVideoAsync(BaseRestClient client, string id)
        {
            var model = await client.RestClient.GetVideoAsync(id);
            var entity = new RestVideo(client, model.Id);
            entity.Update(model);
            return entity;
        }

        #endregion
    }
}
