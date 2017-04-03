﻿using System;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.Token;

namespace NTwitch.Rest
{
    public class RestTokenInfo
    {
        /// <summary> The oauth token of this session. </summary>
        public string Token { get; private set; }
        /// <summary> True if the specified token is valid </summary>
        public bool IsValid { get; private set; }
        /// <summary> The authorized user's name </summary>
        public string Username { get; private set; }
        /// <summary> The authorized user's id </summary>
        public ulong UserId { get; private set; }
        /// <summary> The client id of the authorized application </summary>
        public string ClientId { get; private set; }
        /// <summary> Information about the authorized oauth token </summary>
        public RestAuthorization Authorization { get; private set; } = new RestAuthorization();

        internal RestTokenInfo(string token)
        {
            Token = token;
        }

        internal static RestTokenInfo Create(Model model, string token)
        {
            var entity = new RestTokenInfo(token);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            IsValid = model.IsValid;
            Username = model.Username;
            UserId = model.UserId;
            ClientId = model.ClientId;
            if (model.Authorization != null)
                Authorization.Update(model.Authorization);
        }

        public virtual Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
