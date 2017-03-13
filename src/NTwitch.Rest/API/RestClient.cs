﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    internal class RestClient : IDisposable
    {
        private HttpClient _client = null;
        
        private TokenType _tokenType;
        private string _host;
        private string _token;
        private bool _disposed = false;

        public RestClient(TwitchRestConfig config, TokenType type, string token)
        {
            _host = config.RestHost;
            _tokenType = type;
            _token = token;
        }

        public async Task<RestResponse> SendAsync(HttpRequestMessage message)
        {
            EnsureClientExists();

            var reply = await _client.SendAsync(message);

            try
            {
                reply.EnsureSuccessStatusCode();
            } catch (HttpRequestException ex)
            {
                throw new HttpException(reply.StatusCode, ex);
            }
            
            var content = await reply.Content.ReadAsStringAsync();
            return new RestResponse(reply.StatusCode, content);
        }

        private void EnsureClientExists()
        {
            if (_client == null)
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(_host);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
                client.DefaultRequestHeaders.Add("User-Agent", TwitchConfig.UserAgent);

                if (_tokenType == TokenType.ClientId)
                    client.DefaultRequestHeaders.Add("Client-ID", _token);
                if (_tokenType == TokenType.Oauth)
                    client.DefaultRequestHeaders.Add("Authorization", $"OAuth {_token}");

                _client = client;
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
