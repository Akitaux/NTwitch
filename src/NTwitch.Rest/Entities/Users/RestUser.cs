﻿using System;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.User;

namespace NTwitch.Rest
{
    public class RestUser : RestSimpleUser, IUser
    {
        /// <summary> The date and time this user was created </summary>
        public DateTime CreatedAt { get; private set; }
        /// <summary> The date and time this user was last updated </summary>
        public DateTime UpdatedAt { get; private set; }
        /// <summary> This user's type </summary>
        public string Type { get; private set; }
        /// <summary> This user's profile description </summary>
        public string Bio { get; private set; }
        
        internal RestUser(BaseTwitchClient client, ulong id, string name) 
            : base(client, id, name) { }

        internal new static RestUser Create(BaseTwitchClient client, Model model)
        {
            var entity = new RestUser(client, model.Id, model.Name);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);
            CreatedAt = model.CreatedAt;
            UpdatedAt = model.UpdatedAt;
            Type = model.Type;
            Bio = model.Bio;
        }

        /// <summary> Get the most recent version of this entity </summary>
        public virtual async Task UpdateAsync()
        {
            var model = await Client.ApiClient.GetUserAsync(Id, null);
            Update(model);
        }
    }
}
