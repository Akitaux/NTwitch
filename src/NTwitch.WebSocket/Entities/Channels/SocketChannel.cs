﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTwitch.WebSocket
{
    public class SocketChannel : SocketEntity, IChannel
    {
        public string BroadcasterLanguage { get; }
        public DateTime CreatedAt { get; }
        public string DisplayName { get; }
        public int FollowerCount { get; }
        public string Game { get; }
        public string Language { get; }
        public string LogoUrl { get; }
        public bool IsMature { get; }
        public string Name { get; }
        public bool IsPartner { get; }
        public string ProfileBannerUrl { get; }
        public string ProfileBackgroundColor { get; }
        public string Status { get; }
        public DateTime UpdatedAt { get; }
        public string Url { get; }
        public string VideoBannerUrl { get; }
        public int ViewCount { get; }

        internal SocketChannel(TwitchSocketClient client, ulong id) : base(client, id) { }

        public Task GetFollowersAsync(bool descending = true, TwitchPageOptions page = null)
        {
            throw new NotImplementedException();
        }

        public Task GetTeamsAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetVideosAsync()
        {
            throw new NotImplementedException();
        }
    }
}
