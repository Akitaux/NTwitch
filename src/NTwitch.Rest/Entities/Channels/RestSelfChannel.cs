﻿using System;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.SelfChannel;

namespace NTwitch.Rest
{
    public class RestSelfChannel : RestChannel
    {
        public string Email { get; private set; }
        public string StreamKey { get; private set; }

        internal RestSelfChannel(BaseRestClient client, ulong id) 
            : base(client, id) { }

        internal static RestSelfChannel Create(BaseRestClient client, Model model)
        {
            var entity = new RestSelfChannel(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Email = model?.Email ?? Email;
            StreamKey = model.StreamKey;
            base.Update(model);
        }

        public override async Task UpdateAsync()
        {
            var entity = await Client.RestClient.GetCurrentChannelAsync().ConfigureAwait(false);
            Update(entity);
        }

        // Channels
        public Task ModifyAsync(Action<ModifyChannelParams> options)
            => ChannelHelper.ModifyChannelAsync(this, options);
    }
}
