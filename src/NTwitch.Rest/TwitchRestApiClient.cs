﻿using Newtonsoft.Json;
using NTwitch.Rest.Queue;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTwitch.Rest.API
{
    internal class TwitchRestApiClient : IDisposable
    {
        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();

        protected readonly JsonSerializer _serializer;
        protected readonly SemaphoreSlim _stateLock;
        private readonly RestClientProvider RestClientProvider;

        protected bool _disposed;
        private CancellationTokenSource _loginCancelToken;

        //public RetryMode DefaultRetryMode { get; }
        public string UserAgent { get; }
        public string ClientId { get; }

        public LoginState LoginState { get; private set; }
        internal IRestClient RestClient { get; private set; }
        internal string AuthToken { get; private set; }
        internal ulong? CurrentUserId { get; set; }

        public TwitchRestApiClient(RestClientProvider restClientProvider, string clientId, string userAgent, JsonSerializer serializer = null)
        {
            RestClientProvider = restClientProvider;
            UserAgent = userAgent;
            ClientId = clientId;
            _serializer = serializer ?? new JsonSerializer { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" };
            //DefaultRetryMode = defaultRetryMode;
            
            _stateLock = new SemaphoreSlim(1, 1);

            SetBaseUrl(TwitchConfig.APIUrl);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    (RestClient as IDisposable)?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);

        internal void SetBaseUrl(string baseUrl)
        {
            RestClient = RestClientProvider(baseUrl);
            RestClient.SetHeader("Accept", $"application/vnd.twitchtv.v{TwitchConfig.APIVersion}+json");
            RestClient.SetHeader("UserAgent", UserAgent);
            if (ClientId != null)
                RestClient.SetHeader("Client-ID", ClientId);
        }

        public async Task LoginAsync(string token, RequestOptions options = null)
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(token, options).ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }

        private async Task LoginInternalAsync(string token, RequestOptions options = null)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                _loginCancelToken = new CancellationTokenSource();
                AuthToken = null;

                RestClient.SetHeader("Authorization", $"OAuth {token}");
                RestClient.SetCancelToken(_loginCancelToken.Token);

                AuthToken = token;
                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternalAsync().ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }

        private async Task LogoutInternalAsync()
        {
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            try { _loginCancelToken?.Cancel(false); }
            catch { }

            RestClient.SetHeader("Authorization", null);
            await DisconnectInternalAsync().ConfigureAwait(false);

            CurrentUserId = null;
            LoginState = LoginState.LoggedOut;
        }

        internal virtual Task ConnectInternalAsync() => Task.Delay(0);
        internal virtual Task DisconnectInternalAsync() => Task.Delay(0);

        public Task SendAsync(RequestBuilder builder, RequestOptions options = null)
            => SendAsync(builder.Method, builder.Endpoint, options);
        public async Task SendAsync(string method, string endpoint, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            var request = new RestRequest(RestClient, method, endpoint, options);
            await SendInternalAsync(method, endpoint, request).ConfigureAwait(false);
        }

        public Task<TResponse> SendAsync<TResponse>(RequestBuilder builder, RequestOptions options = null)
            => SendAsync<TResponse>(builder.Method, builder.Endpoint, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            var request = new RestRequest(RestClient, method, endpoint, options);
            return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
        }

        public Task SendJsonAsync(JsonRequestBuilder builder, RequestOptions options = null)
            => SendJsonAsync(builder.Method, builder.Endpoint, builder.Payload, options);
        public async Task SendJsonAsync(string method, string endpoint, object payload, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            var json = payload != null ? SerializeJson(payload) : null;
            var request = new JsonRestRequest(RestClient, method, endpoint, json, options);
            await SendInternalAsync(method, endpoint, request).ConfigureAwait(false);
        }

        public Task<TResponse> SendJsonAsync<TResponse>(JsonRequestBuilder builder, RequestOptions options = null)
            => SendJsonAsync<TResponse>(builder.Method, builder.Endpoint, builder.Payload, options);
        public async Task<TResponse> SendJsonAsync<TResponse>(string method, string endpoint, object payload, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            var json = payload != null ? SerializeJson(payload) : null;
            var request = new JsonRestRequest(RestClient, method, endpoint, json, options);
            return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
        }

        private async Task<System.IO.Stream> SendInternalAsync(string method, string endpoint, RestRequest request)
        {
            if (!request.Options.IgnoreState)
                CheckState();
            //if (request.Options.RetryMode == null)
            //    request.Options.RetryMode = DefaultRetryMode;

            var stopwatch = Stopwatch.StartNew();
            var response = await request.SendAsync().ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return response.Body;
        }

        protected void CheckState()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("Client is not logged in.");
        }

        protected string SerializeJson(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }

        protected T DeserializeJson<T>(System.IO.Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }

        protected static double ToMilliseconds(Stopwatch stopwatch) 
            => Math.Round((double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000.0, 2);

        // Tokens
        public async Task<TokenCollection> ValidateTokenAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<TokenCollection>("GET", "", options).ConfigureAwait(false);
        }

        // Channels
        public async Task<Channel> GetMyChannelAsync(RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<Channel>("GET", "channel", options).ConfigureAwait(false);
        }

        public async Task<Channel> GetChannelAsync(ulong channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<Channel>("GET", $"channels/{channelId}", options).ConfigureAwait(false);
        }

        public async Task<Channel> ModifyChannelAsync(ulong channelId, ModifyChannelParams changes, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendJsonAsync<Channel>(new ModifyChannelRequest(channelId, changes), options).ConfigureAwait(false);
        }

        public async Task<UserCollection> GetChannelEditorsAsync(ulong channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<UserCollection>("GET", $"channels/{channelId}/editors", options).ConfigureAwait(false);
        }

        // Chat
        public async Task<CheerCollection> GetCheersAsync(ulong? channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<CheerCollection>(new GetCheersRequest(channelId), options).ConfigureAwait(false);
        }

        public async Task<ChatBadges> GetChatBadgesAsync(ulong channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<ChatBadges>("GET", $"chat/{channelId}/badges", options).ConfigureAwait(false);
        }

        // Communities
        public async Task<Community> GetCommunityAsync(string communityId, bool isName, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<Community>(new GetCommunityRequest(communityId, isName), options).ConfigureAwait(false);
        }

        public async Task<CommunityCollection> GetTopCommunitiesAsync(PageOptions paging, RequestOptions options)       // Paging is unused because I'm lazy
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<CommunityCollection>("GET", "communities/top", options).ConfigureAwait(false);
        }

        public async Task<CommunityPermissions> GetCommunityPermissionsAsync(string communityId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<CommunityPermissions>("GET", $"communities/{communityId}/permissions", options).ConfigureAwait(false);
        }

        public async Task SendCommunityReportAsync(string communityId, ulong channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendAsync(new SendCommunityReportRequest(communityId, channelId), options).ConfigureAwait(false);
        }

        public async Task ModifyCommunityAsync(string communityId, ModifyCommunityParams changes, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendJsonAsync("PUT", $"communitites/{communityId}", changes, options).ConfigureAwait(false);
        }
        
        public async Task SetCommunityAvatarAsync(string communityId, string imageBase64, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendJsonAsync(new SetCommunityAvatarRequest(communityId, imageBase64), options).ConfigureAwait(false);
        }

        public async Task RemoveCommunityAvatarAsync(string communityId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendAsync("DELETE", $"communities/{communityId}/images/avatar", options).ConfigureAwait(false);
        }

        public async Task SetCommunityCoverAsync(string communityId, string imageBase64, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendAsync(new SetCommunityCoverRequest(communityId, imageBase64), options).ConfigureAwait(false);
        }

        public async Task RemoveCommunityCoverAsync(string communityId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendAsync("DELETE", $"communities/{communityId}/images/cover", options).ConfigureAwait(false);
        }

        //
        // You took a break here don't forget ;)
        //

        // Subscribers
        public async Task<Subscription> GetSubscriptionAsync(ulong channelId, ulong userId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<Subscription>("GET", $"channels/{channelId}/subscriptions/{userId}", options).ConfigureAwait(false);
        }

        public async Task<SubscriptionCollection> GetSubscribersAsync(ulong channelId, bool ascending, PageOptions paging, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            paging = PageOptions.CreateOrClone(paging);
            return await SendAsync<SubscriptionCollection>(new GetSubscribersRequest(channelId, ascending, paging), options).ConfigureAwait(false);
        }

        // Teams
        public async Task<TeamCollection> GetChannelTeamsAsync(ulong channelId, RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<TeamCollection>("GET", $"channels/{channelId}/teams", options).ConfigureAwait(false);
        }

        // Users
        public async Task<User> GetMyUserAsync(RequestOptions options)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<User>("GET", "user", options).ConfigureAwait(false);
        }
    }
}