﻿using System.Reflection;

namespace NTwitch
{
    public class TwitchConfig
    {
        public static string Version { get; } =
            typeof(TwitchConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(TwitchConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
            "Unknown";

        public static string UserAgent { get; } = $"NTwitchApp (https://github.com/Aux/NTwitch, v{Version})";
        public static readonly string APIUrl = $"https://api.twitch.tv/kraken/";

        public const int APIVersion = 5;
        public const int DefaultRequestTimeout = 15000;
        
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
    }
}
