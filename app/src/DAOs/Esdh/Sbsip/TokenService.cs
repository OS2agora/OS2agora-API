using BallerupKommune.Operations.ApplicationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Esdh.Sbsip
{
    public class TokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IOptions<SbsipOptions> _sbsipOptions;
        private readonly IConfiguration _configuration;

        public TokenService(HttpClient httpClient, IOptions<SbsipOptions> sbsipOptions,
            IConfiguration configuration, ILogger<TokenService> logger)
        {
            _sbsipOptions = sbsipOptions;
            _configuration = configuration;
            _logger = logger;

            httpClient.BaseAddress = new Uri(_sbsipOptions.Value.TokenServiceBaseAddress);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            _httpClient = httpClient;
        }

        public async Task<string> GetAccessToken()
        {
            string stringJson;
            try
            {
                stringJson = await RequestToken();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"SBSIP (Token Service): HttpRequestException caught with exception: {exception}");
                throw new Exception(exception.Message);
            }
            catch (AggregateException exception)
            {
                _logger.LogError($"SBSIP (Token Service): HttpRequestException caught with exception: {exception}");
                throw new Exception(exception.Message);
            }

            var token = JObject.Parse(stringJson).GetValue("access_token");
            return token.ToString();
        }

        private async Task<string> RequestToken()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_sbsipOptions.Value.TokenRequestUri, UriKind.Relative),
                Content = new FormUrlEncodedContent(ConstructRequestBodyContent())
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        private IEnumerable<KeyValuePair<string, string>> ConstructRequestBodyContent()
        {
            var grantType = _sbsipOptions.Value.GrantType;
            var clientId = _sbsipOptions.Value.ClientId;

            var clientSecret = _sbsipOptions.Value.ClientSecret;

            var requestBodyContent = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            return requestBodyContent;
        }
    }
}