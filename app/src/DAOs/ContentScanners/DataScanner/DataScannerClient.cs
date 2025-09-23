using BallerupKommune.DAOs.ContentScanners.DataScanner.DTOs;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace BallerupKommune.DAOs.ContentScanners.DataScanner
{
    public class DataScannerClient
    {
        protected readonly HttpClient HttpClient;
        private readonly ILogger<DataScannerClient> _logger;

        public DataScannerClient(HttpClient httpClient, IOptions<DataScannerOptions> dataScannerOptions,
            ILogger<DataScannerClient> logger)
        {
            httpClient.BaseAddress = new Uri(dataScannerOptions.Value.BaseAddress);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", dataScannerOptions.Value.Token);
            HttpClient = httpClient;
            _logger = logger;
        }

        public Task<DataScannerResult> ScanFileContent(byte[] fileContent, string mimeType, string fileName)
        {
            var base64EncodedContent = Convert.ToBase64String(fileContent);

            return ScanContent(base64EncodedContent, mimeType, fileName);
        }

        public Task<DataScannerResult> ScanTextContent(string textContent)
        {
            var stringAsByteArray = Encoding.UTF8.GetBytes(textContent);
            var base64EncodedContent = Convert.ToBase64String(stringAsByteArray);

            return ScanContent(base64EncodedContent, "text/plain", "text-content");
        }

        private async Task<DataScannerResult> ScanContent(string contentAsBase64, string mimeType, string name)
        {
            var scanDto = new ScanDto
            {
                Source = new SourceDto
                {
                    Type = "data",
                    Content = contentAsBase64,
                    Mime = mimeType,
                    Name = name
                },
                Rule = new RuleDto
                {
                    Type = "cpr"
                }
            };

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(scanDto, serializerSettings), Encoding.UTF8,
                "application/json");

            try
            {
                var response = await HttpClient.PostAsync("scan/1", postContent);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"DataScanner: result: {result}");
                // Result is not valid JSON, it could look like this {...}{...} and sometimes like this {...}\n{...}
                // It is missing either [] + "," between objects or we need to split each object and treat them separately
                var validJson = "[" + result.Replace("}{", "},{").Replace("}\n{", "},{") + "]";

                var scanResult = JsonConvert.DeserializeObject<List<ScanResultDto>>(validJson, serializerSettings);
                _logger.LogInformation($"DataScanner: deserialized data: {scanResult}");

                if (scanResult == null)
                {
                    _logger.LogError($"DataScanner: No result");
                    return DataScannerResult.Error;
                }

                if (scanResult.Any(x => x.Matched))
                {
                    _logger.LogInformation($"DataScanner: found \"personal data\"");
                    return DataScannerResult.Dirty;
                }

                return DataScannerResult.Clean;
            }
            catch (Exception e)
            {
                _logger.LogError($"DataScanner: Unknown exception caught: {e.Message}");
                return DataScannerResult.Error;
            }
        }
    }
}