using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Agora.DAOs.UserDataEnrichment.CVR.DTOs;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;

namespace Agora.DAOs.UserDataEnrichment.CVR;

public class CvrInformationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CvrInformationClient> _logger;
    private readonly IOptions<CvrInformationOptions> _options;

    public CvrInformationClient(HttpClient httpClient, ILogger<CvrInformationClient> logger, IOptions<CvrInformationOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options;
        InitializeHttpClient();
    }

    private void InitializeHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_options.Value.HostName);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
    }

    public async Task<HentCvrDataDto> GetCvrData(string cvr)
    {
        HttpResponseMessage response = null;
        try
        {
            var requestUri = $"CVR/HentCVRData/1/rest/hentVirksomhedMedCVRNummer?pCVRNummer={cvr}";
            response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var contentAsString = await response.Content.ReadAsStringAsync();
            var dto = HentCvrDataDto.FromJson(contentAsString);
            return dto;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{HttpClient}: Exception caught: {errorMsg}", nameof(CvrInformationClient), e.Message);
            if (response?.Content != null)
            {
                _logger.LogError("{HttpClient}: response_body: {response_body}", nameof(CvrInformationClient), await response.Content.ReadAsStringAsync());
            }

            return null;
        }
    }
}