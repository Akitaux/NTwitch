﻿using Model = NTwitch.Rest.API.Subscription;

namespace NTwitch.Rest
{
    public class RestChannelSubscription : RestSubscription
    {
        public RestChannel Channel { get; private set; }

        internal RestChannelSubscription(BaseRestClient client) 
            : base(client) { }

        internal new static RestChannelSubscription Create(BaseRestClient client, Model model)
        {
            var entity = new RestChannelSubscription(client);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            Channel = new RestChannel(Client, model.Channel.Id);
            Channel.Update(model.Channel);
            base.Update(model);
        }
    }
}
