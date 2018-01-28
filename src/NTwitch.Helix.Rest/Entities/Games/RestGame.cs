﻿using System.Linq;
using System.Threading.Tasks;
using Model = NTwitch.Helix.API.Game;

namespace NTwitch.Helix.Rest
{
    public class RestGame : RestNamedEntity<ulong>
    {
        /// <summary> The url to this game's box art image </summary>
        public string BoxArtUrl { get; private set; }
        
        internal RestGame(BaseTwitchClient twitch, ulong id, string name)
            : base(twitch, id, name) { }
        internal static RestGame Create(BaseTwitchClient twitch, Model model)
        {
            var entity = new RestGame(twitch, model.Id, model.Name);
            entity.Update(model);
            return entity;
        }
        internal virtual void Update(Model model)
        {
            BoxArtUrl = model.BoxArtUrl;
        }

        /// <summary> Update this object to the most recent information available </summary>
        public async Task UpdateAsync(RequestOptions options = null)
        {
            var models = await Twitch.ApiClient.GetGamesAsync(new[] { Id }, null, options: options).ConfigureAwait(false);
            Update(models.SingleOrDefault());
        }
    }
}
