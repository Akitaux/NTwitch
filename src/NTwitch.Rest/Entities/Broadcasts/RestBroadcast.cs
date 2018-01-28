﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.Broadcast;

namespace NTwitch.Rest
{
    public class RestBroadcast : RestEntity<ulong>, IUpdateable, IEquatable<RestBroadcast>
    {
        /// <summary> Date and time when this stream was created </summary>
        public DateTime CreatedAt { get; private set; }
        /// <summary> The name of the game being streamed </summary>
        public string Game { get; private set; }
        /// <summary> The delay in seconds of this stream </summary>
        public int Delay { get; private set; }
        /// <summary> The number of viewers currently watching this stream </summary>
        public uint Viewers { get; private set; }
        /// <summary> The height of this stream's video </summary>
        public uint VideoHeight { get; private set; }
        /// <summary> The average fps of this stream's video </summary>
        public double AverageFps { get; private set; }
        /// <summary> True if this stream is pre-recorded </summary>
        public bool IsPlaylist { get; private set; }
        /// <summary> Preview images for this stream </summary>
        public IReadOnlyDictionary<string, string> Previews { get; private set; }
        /// <summary> The channel this stream is associated with </summary>
        public RestChannel Channel { get; private set; }

        internal RestBroadcast(BaseTwitchClient client, ulong id)
            : base(client, id) { }

        public bool Equals(RestBroadcast other)
            => Id == other.Id;
        public override string ToString()
            => Channel.ToString();

        internal static RestBroadcast Create(BaseTwitchClient client, Model model)
        {
            var entity = new RestBroadcast(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Channel = RestChannel.Create(Client, model.Channel);

            CreatedAt = model.CreatedAt;
            Game = model.Game;
            Delay = model.Delay;
            Viewers = model.Viewers;
            VideoHeight = model.VideoHeight;
            AverageFps = model.AverageFps;
            IsPlaylist = model.IsPlaylist;
            Previews = model.Previews;
        }
        
        /// <summary> Get the most recent information for this entity </summary>
        public virtual Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
