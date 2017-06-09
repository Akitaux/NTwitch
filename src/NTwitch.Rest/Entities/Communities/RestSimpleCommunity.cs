﻿using Model = NTwitch.Rest.API.Community;

namespace NTwitch.Rest
{
    public class RestSimpleCommunity : RestEntity<string>, ISimpleCommunity
    {
        /// <summary> The name of this community </summary>
        public string Name { get; private set; }
        /// <summary> The url to the avatar image of this community </summary>
        public string AvatarUrl { get; private set; }

        internal RestSimpleCommunity(BaseTwitchClient client, string id)
            : base(client, id) { }
        
        internal static RestSimpleCommunity Create(BaseTwitchClient client, Model model)
        {
            var entity = new RestSimpleCommunity(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Name = model.Name;
            AvatarUrl = model.AvatarImageUrl;
        }

        internal void Update(RestSimpleCommunity community)
        {
            Name = community.Name;
            AvatarUrl = community.AvatarUrl;
        }

        // IEqualityComparer
        public bool Equals(ISimpleCommunity x, ISimpleCommunity y) => x.Id == y.Id;
        public int GetHashCode(ISimpleCommunity obj) => obj.GetHashCode();
    }
}
