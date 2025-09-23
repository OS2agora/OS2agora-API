using BallerupKommune.Operations.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.OAuth2
{
    public class OAuth2Client
    {
        protected readonly HttpClient HttpClient;
        private readonly ILogger<OAuth2Client> _logger;

        public OAuth2Client(HttpClient httpClient, ILogger<OAuth2Client> logger)
        {
            HttpClient = httpClient;
            _logger = logger;
        }

        public async Task<OAuth2TokenResponse> RequestAccessTokenCode(string code, string codeVerifier,
            Operations.ApplicationOptions.OAuth2Client oauthClientOptions)
        {
            var tokenEndpoint = new Uri(oauthClientOptions.TokenEndpoint);
            var redirectUri = new Uri(oauthClientOptions.RedirectUri);

            var clientId = oauthClientOptions.ClientId;
            var clientSecret = oauthClientOptions.ClientSecret;

            var basicAuthValueAsByteArray = new UTF8Encoding().GetBytes($"{clientId}:{clientSecret}");
            var basicAuthValueAsBase64String = Convert.ToBase64String(basicAuthValueAsByteArray);

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthValueAsBase64String);

            var postContent = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"redirect_uri", redirectUri.AbsoluteUri},
                {"code", code},
                {"code_verifier", codeVerifier}
            };

            var encodedPostContent = new FormUrlEncodedContent(postContent);

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync(tokenEndpoint, encodedPostContent);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var oauth2TokenResponse = JsonConvert.DeserializeObject<OAuth2TokenResponse>(result);
                return oauth2TokenResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"OAauth2Client: Exception caught: {e.Message}");
                if (response?.Content != null)
                {
                    _logger.LogError($"request_body: {await response.Content.ReadAsStringAsync()}");
                }
                throw;
            }
        }
    }
}