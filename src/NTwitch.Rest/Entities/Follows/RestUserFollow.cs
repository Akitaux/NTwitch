﻿using Model = NTwitch.Rest.API.Follow;

namespace NTwitch.Rest
{
    public class RestUserFollow : RestFollow
    {
        /// <summary> The user associated with this follow </summary>
        public RestUser User { get; private set; }

        internal RestUserFollow(BaseTwitchClient client) : base(client) { }

        internal new static RestUserFollow Create(BaseTwitchClient client, Model model)
        {
            var entity = new RestUserFollow(client);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);
            User = RestUser.Create(Client, model.User);
        }
    }
}
