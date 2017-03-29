﻿using NTwitch.Pubsub;
using System;
using System.Threading.Tasks;

namespace NTwitch.Tests
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private TwitchPubsubClient _client;

        public async Task Start()
        {
            _client = new TwitchPubsubClient(new TwitchPubsubConfig()
            {
                LogLevel = LogLevel.Info
            });

            _client.Log += OnLogAsync;
            _client.WhisperReceived += OnWhisperReceivedAsync;

            await _client.LoginAsync(AuthMode.Oauth, "");
            await _client.AddWhispersAsync(123, 1321, 123);
            
            await Task.Delay(-1);
        }

        private Task OnWhisperReceivedAsync()
        {
            throw new NotImplementedException();
        }

        private Task OnLogAsync(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}