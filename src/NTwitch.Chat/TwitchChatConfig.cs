﻿using NTwitch.Pubsub;

namespace NTwitch.Chat
{
    public class TwitchChatConfig : TwitchPubsubConfig
    {
        /// <summary> Allow the authenticated user to speak in channels without the moderator permission. </summary>
        public bool CanSpeakWithoutMod { get; set; } = false;

        /// <summary> Custom client provider for chat requests </summary>
        public SocketClient ChatProvider { get; set; } = null;
        /// <summary> The host to connect to when making chat requests </summary>
        public string ChatHost { get; set; } = "irc.chat.twitch.tv";
        public int ChatPort { get; set; } = 6667;

        public bool RequestTags { get; set; } = true;
        public bool RequestCommands { get; set; } = true;
        public bool RequestMembership { get; set; } = true;
    }
}
