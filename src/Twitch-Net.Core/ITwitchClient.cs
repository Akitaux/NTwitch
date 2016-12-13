﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twitch
{
    public interface ITwitchClient
    {
        ConnectionState ConnectionState { get; }

        Task LoginAsync(string token = null);
    }
}
