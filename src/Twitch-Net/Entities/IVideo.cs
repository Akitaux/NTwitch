﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twitch
{
    public interface IVideo
    {
        string Title { get; }
        string Description { get; }
        uint BroadcastId { get; }
        string Status { get; }
        string Id { get; }
        string[] Tags { get; }
        DateTime RecordedAt { get; }
        string Game { get; }
        int Length { get; }
        string PreviewUrl { get; }
        string Url { get; set; }
        int Views { get; set; }
        BroadcastType Type { get; }
        string[] Links { get; }
        IChannel Channel { get; }
    }
}