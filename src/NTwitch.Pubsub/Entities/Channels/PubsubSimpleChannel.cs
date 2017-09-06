﻿using NTwitch.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitsModel = NTwitch.Pubsub.API.BitsMessageEvent;
using EventModel = NTwitch.Pubsub.API.BaseEvent;

namespace NTwitch.Pubsub
{
    public class PubsubSimpleChannel : PubsubEntity<ulong>, ISimpleChannel
    {
        public string Name { get; private set; }
        public string DisplayName => Name;
        
        internal PubsubSimpleChannel(TwitchPubsubClient client, ulong id)
            : base(client, id) { }

        public bool Equals(ISimpleChannel other)
            => Id == other.Id;
        public override string ToString()
            => Name;

        internal static PubsubSimpleChannel Create(TwitchPubsubClient client, EventModel model)
        {
            var entity = new PubsubSimpleChannel(client, model.ChannelId);
            entity.Update(model);
            return entity;
        }

        internal static PubsubSimpleChannel Create(TwitchPubsubClient client, BitsModel model)
        {
            var entity = new PubsubSimpleChannel(client, model.Data.ChannelId);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(EventModel model)
        {
            Name = model.ChannelName;
        }

        internal virtual void Update(BitsModel model)
        {
            Name = model.Data.ChannelName;
        }

        // Channels
        /// <summary> Change properties of this channel </summary>
        public async Task<RestChannel> ModifyAsync(Action<ModifyChannelParams> changes, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(Client, this, changes, options);
            return RestChannel.Create(Client, model);
        }

        // Chat
        /// <summary> Get cheer badges for this channel </summary>
        public Task<IReadOnlyCollection<RestCheerInfo>> GetCheersAsync(RequestOptions options = null)
            => ClientHelper.GetCheersAsync(Client, Id, options);
        /// <summary> Get chat badges for this channel </summary>
        public Task<RestChatBadges> GetChatBadgesAsync(RequestOptions options = null)
            => ChannelHelper.GetChatBadgesAsync(Client, Id, options);

        // Streams
        /// <summary> Get this channel's stream information, if available </summary>
        public Task<RestStream> GetStreamAsync(StreamType type = StreamType.Live, RequestOptions options = null)
            => ClientHelper.GetStreamAsync(Client, Id, type, options);

        // Teams
        /// <summary> Get all teams this channel is a member of </summary>
        public Task<IReadOnlyCollection<RestSimpleTeam>> GetTeamsAsync(RequestOptions options = null)
            => ChannelHelper.GetTeamsAsync(Client, Id, options);

        // Users
        /// <summary> Get information about this channel's user </summary>
        public Task<RestUser> GetUserAsync(RequestOptions options = null)
            => ClientHelper.GetUserAsync(Client, Id, options);
        /// <summary> Get information about this channel's user, if authenticated </summary>
        public Task<RestSelfUser> GetSelfUserAsync(RequestOptions options = null)
            => ClientHelper.GetCurrentUserAsync(Client, options);

        // Users
        /// <summary> Get all users following this channel </summary>
        public Task<IReadOnlyCollection<RestUserFollow>> GetFollowersAsync(bool ascending = false, PageOptions paging = null, RequestOptions options = null)
            => ChannelHelper.GetFollowersAsync(Client, Id, ascending, paging, options);
        /// <summary> Get all users authorized as an editor on this channel </summary>
        public Task<IReadOnlyCollection<RestUser>> GetEditorsAsync(RequestOptions options = null)
            => ChannelHelper.GetEditorsAsync(Client, Id, options);
        /// <summary> Get all users subscribed to this channel </summary>
        public Task<IReadOnlyCollection<RestUserSubscription>> GetSubscribersAsync(bool ascending = false, PageOptions paging = null, RequestOptions options = null)
            => ChannelHelper.GetSubscribersAsync(Client, Id, ascending, paging, options);
        /// <summary> Get a specific user subscriber by id </summary>
        public Task<RestUserSubscription> GetSubscriberAsync(ulong userId, RequestOptions options = null)
            => ChannelHelper.GetSubscriberAsync(Client, Id, userId, options);

        // Videos
        /// <summary>  </summary>
        public Task<IReadOnlyCollection<RestVideo>> GetVideosAsync(PageOptions paging = null, RequestOptions options = null)    // Add parameters at some point
            => ChannelHelper.GetVideosAsync(Client, Id, paging, options);
        /// <summary>  </summary>
        public Task<IReadOnlyCollection<RestClip>> GetClipsAsync(bool istrending = false, PageOptions paging = null, RequestOptions options = null)
            => ClientHelper.GetFollowedClipsAsync(Client, Id, istrending, paging, options);


    }
}
