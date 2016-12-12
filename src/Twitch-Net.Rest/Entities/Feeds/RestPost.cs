﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twitch.Rest
{
    public class RestPost : IPost
    {
        public IPostUser Author { get; }
        public string Body { get; }
        public DateTime CreatedAt { get; }
        public string[] Emotes { get; }
        public uint Id { get; }
        public bool IsDeleted { get; }
        public IEnumerable<PostReaction> Reactions { get; }

        public Task CreateReactionAsync(uint id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteReactionAsync(uint id)
        {
            throw new NotImplementedException();
        }
    }
}
