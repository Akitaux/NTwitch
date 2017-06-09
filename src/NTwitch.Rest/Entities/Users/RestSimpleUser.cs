﻿using Model = NTwitch.Rest.API.User;

namespace NTwitch.Rest
{
    public class RestSimpleUser : RestEntity<ulong>, ISimpleUser
    {
        /// <summary> The url for this user's logo </summary>
        public string LogoUrl { get; private set; }
        /// <summary> The name of this user </summary>
        public string Name { get; private set; }
        /// <summary> The display name of this user </summary>
        public string DisplayName { get; private set; }

        internal RestSimpleUser(BaseTwitchClient client, ulong id) 
            : base(client, id) { }

        internal static RestSimpleUser Create(BaseTwitchClient client, Model model)
        {
            var entity = new RestSimpleUser(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            DisplayName = model.DisplayName;
            Name = model.Name;
        }
        
        public bool Equals(ISimpleUser x, ISimpleUser y)
            => x.Id == y.Id;
        public int GetHashCode(ISimpleUser user)
            => user.GetHashCode();
    }
}
