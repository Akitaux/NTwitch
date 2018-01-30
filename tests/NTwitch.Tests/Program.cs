﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NTwitch.Helix.Rest;

namespace NTwitch.Tests
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private TwitchRestClient _client;

        public async Task Start()
        {
            string token = "";
            string clientId = "";

            _client = new TwitchRestClient(new TwitchRestConfig
            {
                ClientId = clientId,
                LogLevel = LogSeverity.Debug
            });

            await _client.LoginAsync(token);

            var clip = await _client.GetClipAsync("ThisIdDoesNotExist");

            var user = await _client.GetUserAsync("Not a valid user");
            var users = await _client.GetUsersAsync(userNames: new[] { "Not a valid user" });
            
            var broadcasts = await _client.GetBroadcastsAsync(limit: 126, languages: new[] { "en" }).Flatten();

            await Task.Delay(-1);
        }
        
        private Task OnLogAsync(LogMessage msg)
        {
            return Console.Out.WriteLineAsync(msg.ToString());
        }
    }
}