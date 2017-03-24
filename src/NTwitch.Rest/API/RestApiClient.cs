﻿using NTwitch.Rest.Requests;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace NTwitch.Rest
{
    public class RestApiClient : IDisposable
    {
        private RestClient _client;
        private LogManager _log;

        private bool _disposed = false;

        public RestApiClient(TwitchRestConfig config, LogManager log, AuthMode type, string token)
        {
            _log = log;
            _client = new RestClient(config, type, token);
        }

        public Task<RestResponse> SendAsync(string method, string endpoint)
            => SendAsync(new RestRequest(method, endpoint));

        public async Task<RestResponse> SendAsync(RestRequest request)
        {
            await _log.DebugAsync("Rest", $"Attempting {request.Method} /{request.Endpoint}").ConfigureAwait(false);

            var message = request.GetRequest();
            var response = await _client.SendAsync(message);

            await _log.VerboseAsync("Rest", $"{request.Method} /{request.Endpoint} {response.ExecuteTime}ms").ConfigureAwait(false);
            return response;
        }

        #region Misc

        internal async Task<API.Token> ValidateTokenAsync()
        {
            try
            {
                var response = await SendAsync(new ValidateTokenRequest());
                return response.GetBodyAsType<API.TokenCollection>().Token;
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401)
            {
                throw new AuthenticationException("Token is invalid.");
            }
        }

        internal async Task<API.IngestCollection> GetIngestsAsync()
        {
            try
            {
                var response = await SendAsync("GET", $"ingests");
                return response.GetBodyAsType<API.IngestCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        #endregion
        #region Users

        internal async Task<API.User> GetCurrentUserAsync()
        {
            try
            {
                var response = await SendAsync("GET", "user");
                return response.GetBodyAsType<API.User>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.User> GetUserAsync(ulong id)
        {
            try
            {
                var response = await SendAsync(new GetUserRequest(id));
                return response.GetBodyAsType<API.User>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 422) { return null; }
        }

        internal async Task<API.UserCollection> GetUsersAsync(string[] usernames)
        {
            try
            {
                var response = await SendAsync(new GetUsersRequest(usernames));
                return response.GetBodyAsType<API.UserCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 400) { return null; }
        }

        #endregion
        #region Channels

        internal async Task<API.Channel> GetChannelAsync(ulong id)
        {
            try
            {
                var response = await SendAsync("GET", $"channels/{id}");
                return response.GetBodyAsType<API.Channel>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        internal async Task<API.Channel> GetCurrentChannelAsync()
        {
            try
            {
                var response = await SendAsync("GET", "channel");
                return response.GetBodyAsType<API.Channel>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.CheerCollection> GetCheersAsync(ulong? id)
        {
            try
            {
                var response = await SendAsync(new GetCheersRequest(id));
                return response.GetBodyAsType<API.CheerCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.Channel> ModifyChannelAsync(ulong channelId, Action<ModifyChannelParams> options)
        {
            var changes = new ModifyChannel();
            options.Invoke(changes.Parameters);

            try
            {
                var response = await SendAsync(new ModifyChannelRequest(channelId, changes));
                return response.GetBodyAsType<API.Channel>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        #endregion
        #region Follows

        internal async Task<API.Follow> GetFollowAsync(ulong userId, ulong channelId)
        {
            try
            {
                var response = await SendAsync("GET", $"users/{userId}/follows/channels/{channelId}");
                return response.GetBodyAsType<API.Follow>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        internal async Task<API.FollowCollection> GetFollowsAsync(ulong userId, SortMode sort, bool ascending, uint limit, uint offset)
        {
            try
            {
                var response = await SendAsync(new GetFollowsRequest(userId, sort, ascending, limit, offset));
                return response.GetBodyAsType<API.FollowCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 422) { return null; }
        }

        #endregion
        #region Community

        internal async Task<API.Community> GetCommunityAsync(string id, bool isname)
        {
            try
            {
                string endpoint;
                if (isname)
                    endpoint = $"communities?name={id}";
                else
                    endpoint = $"communities/{id}";
                
                var response = await SendAsync("GET", endpoint);
                return response.GetBodyAsType<API.Community>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        internal async Task<API.CommunityCollection> GetTopCommunitiesAsync(uint limit)
        {
            try
            {
                var response = await SendAsync("GET", $"communities/top");
                return response.GetBodyAsType<API.CommunityCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.CommunityPermissions> GetCommunityPermissionsAsync(string communityId)
        {
            try
            {
                var response = await SendAsync("GET", $"communities/{communityId}/permissions");
                return response.GetBodyAsType<API.CommunityPermissions>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }
        
        internal async Task ModifyCommunityAsync(string communityId, Action<ModifyCommunityParams> options)
        {
            var changes = new ModifyCommunityParams();
            options.Invoke(changes);

            try
            {
                var response = await SendAsync(new ModifyCommunityRequest(communityId, changes));
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 204) { return; }
        }

        internal async Task SetAvatarAsync(string communityId, string imageString)
        {
            try
            {
                var response = await SendAsync(new SetCommunityAvatarRequest(communityId, imageString));
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 204) { return; }
        }

        internal async Task RemoveAvatarAsync(string communityId)
        {
            try
            {
                var response = await SendAsync(new RemoveCommunityAvatarRequest(communityId));
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 204) { return; }
        }

        internal async Task SetCoverAsync(string communityId, string imageString)
        {
            try
            {
                var response = await SendAsync(new SetCommunityCoverRequest(communityId, imageString));
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 204) { return; }
        }

        internal async Task RemoveCoverAsync(string communityId)
        {
            try
            {
                var response = await SendAsync(new RemoveCommunityCoverRequest(communityId));
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 204) { return; }
        }

        internal async Task<API.CommunityCollection> GetCommunityModeratorsAsync(string id)
        {
            try
            {
                var response = await SendAsync("GET", $"communities/{id}/moderators");
                return response.GetBodyAsType<API.CommunityCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.CommunityCollection> GetCommunityBansAsync(string id, uint limit)
        {
            try
            {
                var response = await SendAsync(new GetCommunityBansRequest(id, limit));
                return response.GetBodyAsType<API.CommunityCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        internal async Task<API.CommunityCollection> GetCommunityTimeoutsAsync(string id, uint limit)
        {
            try
            {
                var response = await SendAsync(new GetCommunityTimeoutsRequest(id, limit));
                return response.GetBodyAsType<API.CommunityCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 401) { return null; }
        }

        #endregion
        #region Videos

        internal async Task<API.Video> GetVideoAsync(string id)
        {
            try
            {
                var response = await SendAsync("GET", $"videos/{id}");
                return response.GetBodyAsType<API.Video>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        #endregion
        #region Emotes

        internal async Task<API.EmoteSet> GetEmotesAsync(ulong id)
        {
            try
            {
                var response = await SendAsync("GET", $"users/{id}/emotes");
                return response.GetBodyAsType<API.EmoteSet>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        #endregion
        #region Teams

        internal async Task<API.Team> GetTeamAsync(string name)
        {
            try
            {
                var response = await SendAsync("GET", $"teams/{name}");
                return response.GetBodyAsType<API.Team>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        internal async Task<API.TeamCollection> GetTeamsAsync(uint limit, uint offset)
        {
            try
            {
                var response = await SendAsync("GET", $"teams");
                return response.GetBodyAsType<API.TeamCollection>();
            }
            catch (HttpException ex) when ((int)ex.StatusCode == 404) { return null; }
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }
                
                _disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
