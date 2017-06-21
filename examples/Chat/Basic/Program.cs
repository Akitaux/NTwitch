﻿using NTwitch;
using NTwitch.Chat;
using System;
using System.Threading.Tasks;

namespace Basic
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private TwitchChatClient _client;

        public async Task StartAsync()
        {
            _client = new TwitchChatClient(new TwitchChatConfig()
            {
                LogLevel = LogSeverity.Info
            });
            
            _client.Log += OnLogAsync;
            _client.MessageReceived += OnMessageReceivedAsync;

            Console.Write("Please enter your oauth token: ");
            string token = Console.ReadLine();

            await _client.LoginAsync(token);
            await _client.ConnectAsync();

            Console.Write("Please the name of a channel to join: ");
            string channelName = Console.ReadLine();

            await _client.JoinChannelAsync(channelName);
            await Task.Delay(-1);
        }

        private Task OnMessageReceivedAsync(ChatMessage msg)
        {
            if (msg is ChatNoticeMessage notice)
                return Console.Out.WriteLineAsync($"[{notice.Channel.Name}] {notice.SystemMessage}");
            
            return Console.Out.WriteLineAsync($"[{msg.Channel.Name}] {msg.User.DisplayName ?? msg.User.Name}: {msg.Content}");
        }

        private Task OnLogAsync(LogMessage msg)
            => Console.Out.WriteLineAsync(msg.ToString());
    }
}