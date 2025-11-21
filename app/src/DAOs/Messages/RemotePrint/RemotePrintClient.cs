using Agora.DAOs.Messages.RemotePrint.DTOs;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Agora.DAOs.Messages.RemotePrint
{
    public class RemotePrintClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RemotePrintClient> _logger;
        private readonly IOptions<RemotePrintOptions> _options;

        public RemotePrintClient(HttpClient httpClient, ILogger<RemotePrintClient> logger, IOptions<RemotePrintOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;

            InitializeHttpClient();
        }

        private void InitializeHttpClient()
        {
            if (_options.Value.Disabled)
            {
                return;
            }

            _httpClient.BaseAddress = new Uri(_options.Value.BaseAddress);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        public async Task<bool?> CanReceiveDigitalMail(CanReceiveDigitalPostDto requestDto)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var requestBody = new StringContent(JsonConvert.SerializeObject(requestDto, serializerSettings), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;

            try
            {
                var requestUri = $"api/remotePrint/canReceiveDigitalPost";
                response = await _httpClient.PostAsync(requestUri, requestBody);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<bool?>();
            }
            catch (Exception ex)
            {
                await LogException(ex, response);
                throw;
            }
        }

        public async Task<RemotePrintDeliveryReceiptDto> SendDigitalPost(LetterDto requestDto)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var postBody = new StringContent(JsonConvert.SerializeObject(requestDto, serializerSettings), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                var requestUri = "api/remotePrint/sendDigitalPost";
                response = await _httpClient.PostAsync(requestUri, postBody);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var receipt = JsonConvert.DeserializeObject<RemotePrintDeliveryReceiptDto>(json);
                return receipt;
            }
            catch (Exception ex)
            {
                await LogException(ex, response);
                throw;
            }
        }

        public async Task<List<PkoStatusResponseDto>> GetMessageStatus()
        {
            HttpResponseMessage response = null;
            try
            {
                var requestUri = "api/mailStatus";
                response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var responseList = JsonConvert.DeserializeObject<List<PkoStatusResponseDto>>(json);
                return responseList;
            }
            catch (Exception ex)
            {
                await LogException(ex, response);
                throw;
            }
        }

        private async Task LogException(Exception ex, HttpResponseMessage response)
        {
            _logger.LogError(ex, "{HttpClient}: Exception caught: {errorMsg}", nameof(RemotePrintClient), ex.Message);
            if (response?.Content != null)
            {
                _logger.LogError("{HttpClient}: response_body: {response_body}", nameof(RemotePrintClient), await response.Content.ReadAsStringAsync());
            }
        }
    }
}