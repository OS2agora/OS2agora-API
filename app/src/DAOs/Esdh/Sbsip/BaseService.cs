using BallerupKommune.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using BallerupKommune.Operations.Common.Exceptions;

namespace BallerupKommune.DAOs.Esdh.Sbsip
{
    public class BaseService
    {
        protected readonly HttpClient HttpClient;
        private readonly ILogger<BaseService> _logger;

        private static string _token;

        public BaseService(HttpClient httpClient, IOptions<SbsipOptions> sbsipOptions, TokenService sbsipTokenService, ILogger<BaseService> logger)
        {
            _logger = logger;
            httpClient.BaseAddress = new Uri(sbsipOptions.Value.BaseAddress);
            var isTokenValid = IsTokenValid(httpClient).GetAwaiter().GetResult();

            if (!isTokenValid)
            {
                _logger.LogInformation($"SBSIP (Base Service): Getting new token");
                _token = sbsipTokenService.GetAccessToken().GetAwaiter().GetResult();
                _logger.LogInformation($"SBSIP (Base Service): Token refreshed");
            }
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            HttpClient = httpClient;
        }

        private async Task<bool> IsTokenValid(HttpClient httpClient)
        {
            if (_token == null)
            {
                return false;
            }

			HttpResponseMessage response = null;
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                _logger.LogInformation($"SBSIP (Base Service): IsTokenValid");
                // This used to work - Now it gives a 401 when not authorized.
                response = await httpClient.GetAsync("info/heartbeat/authorized");

                _logger.LogInformation($"SBSIP (Base Service): IsTokenValid - The action was: {(response.IsSuccessStatusCode ? "successful" : "failed")}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogInformation("SBSIP (Base Service): HttpRequestException caught - Trying to refresh token...");
                _logger.LogDebug($"SBSIP (Base Service): HttpRequestException caught with exception: {exception}");
                if (response?.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new SbsipAuthorizationException(exception.Message);
                }
                return false;
            }
            catch (Exception exception)
            {
                _logger.LogWarning("SBSIP (Base Service): Exception caught - Trying to refresh token...");
                _logger.LogDebug($"SBSIP (Base Service): Exception caught with exception: {exception}");
                return false;
            }
        }
    }
}