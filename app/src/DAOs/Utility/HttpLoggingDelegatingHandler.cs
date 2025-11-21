using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Agora.DAOs.Utility
{
    public class HttpLoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<HttpLoggingDelegatingHandler> _logger;

        public HttpLoggingDelegatingHandler(ILogger<HttpLoggingDelegatingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"HTTP {request.Method} {request.RequestUri} sent");
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = null;
            
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation(
                    $"HTTP {request.Method} {request.RequestUri} responded {(int)response.StatusCode} ({response.StatusCode}) in {stopwatch.ElapsedMilliseconds} ms");
                response.EnsureSuccessStatusCode();
                
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    $"HTTP {request.Method} {request.RequestUri} responded in {stopwatch.ElapsedMilliseconds} ms with response {(int?)response?.StatusCode} ({response?.StatusCode}) {response?.Content?.ReadAsStringAsync().Result}");

                if (!Primitives.Logic.Environment.IsProduction())
                {
                    if (request.Method == HttpMethod.Put || request.Method == HttpMethod.Post || request.Method == HttpMethod.Patch)
                    {
                        _logger.LogInformation($"Request body: {request.Content.ReadAsStringAsync().Result}");
                    }
                }
                throw;
            }
        }
    }
}