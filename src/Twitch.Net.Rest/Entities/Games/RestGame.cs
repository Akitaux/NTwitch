﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twitch.Rest
{
    public class RestGame : IGame
    {
        public uint Id { get; }
        public uint GiantBombId { get; }
        public string Name { get; }
        public GameImage BoxArt { get; }
        public GameImage Logo { get; }
    }
}
