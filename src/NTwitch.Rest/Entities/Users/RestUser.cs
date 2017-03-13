﻿using System;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.User;

namespace NTwitch.Rest
{
    public class RestUser : RestEntity<ulong>, IUser
    {
        public string DisplayName { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Bio { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        
        internal RestUser(BaseRestClient client, ulong id) 
            : base(client, id) { }

        internal static RestUser Create(BaseRestClient client, Model model)
        {
            var entity = new RestUser(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            DisplayName = model.DisplayName;
            Name = model.Name;
            Type = model.Type;
            Bio = model.Bio;
            CreatedAt = model.CreatedAt;
            UpdatedAt = model.UpdatedAt;
        }

        public virtual Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
