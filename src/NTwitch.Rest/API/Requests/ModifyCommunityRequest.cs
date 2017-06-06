﻿using Newtonsoft.Json;

namespace NTwitch.Rest
{
    internal class ModifyCommunityRequest : OldRestRequest
    {
        public ModifyCommunityRequest(string token, string communityId, ModifyCommunityParams changes) 
            : base("PUT", $"communities/{communityId}", token)
        {
            JsonBody = JsonConvert.SerializeObject(changes);
        }
    }
}
