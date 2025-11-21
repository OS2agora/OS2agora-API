using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Agora.DAOs.Esdh.Sbsip.V12
{
    public class CaseServiceV12 : BaseService, ICaseServiceV12
    {
        private readonly ILogger<CaseServiceV12> _logger;

        public CaseServiceV12(HttpClient httpClient, IOptions<SbsipOptions> sbsipOptions,
            TokenService sbsipTokenService, ILogger<CaseServiceV12> logger,
            ILogger<BaseService> baseServiceLogger) : base(httpClient, sbsipOptions, sbsipTokenService,
            baseServiceLogger)
        {
            _logger = logger;
        }

        public async Task<SagDtoV10> CreateCase(int userId, string title, int templateId)
        {
            var caseTemplate = new CreateSagFromSkabelonDto
            {
                SagsTitel = title,
                SagsBehandlerID = userId,
                SkabelonId = templateId
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(caseTemplate), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync("sag/template", postContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to create case. UserId=<{UserId}>, Title=<{Title}>, TemplateId=<{TemplateId}>,  ErrorMessage=<{ErrorMessage}>.",
                    userId, title, templateId, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var caseObject = JsonConvert.DeserializeObject<SagDtoV10>(result);
            return caseObject;
        }

        public async Task<SagDtoV10> UpdateHearingOwnerOnCase(Guid caseGuid, BrugerDto userDto)
        {
            var updatedCase = new SagDtoV10
            {
                Behandler = userDto
            };

            var putContent = new StringContent(JsonConvert.SerializeObject(updatedCase), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PutAsync($"sag/{caseGuid}", putContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to update hearingowner on case. CaseGuid=<{CaseGuid}>, ErrorMessage=<{ErrorMessage}>.",
                    caseGuid, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var caseObject = JsonConvert.DeserializeObject<SagDtoV10>(result);
            return caseObject;
        }

        public async Task<List<SagsStatusDto>> GetCaseStatus()
        {
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.GetAsync($"sag/sagStatusList");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to get status on case. ErrorMessage=<{ErrorMessage}>.", exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var caseStatus = JsonConvert.DeserializeObject<List<SagsStatusDto>>(result);
            return caseStatus;
        }

        public async Task<SagDto> CloseCase(Guid caseGuid, int caseStatusId, string comment = null)
        {
            var statusModel = new SagsStatusModel
            {
                SagsStatusID = caseStatusId,
                Kommentar = comment
            };

            var putContent = new StringContent(JsonConvert.SerializeObject(statusModel), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PutAsync($"sag/{caseGuid}/status", putContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    "Failed to close case. CaseGuid=<{CaseGuid}>, CaseStatusId=<{CaseStatusId}>, ErrorMessage=<{ErrorMessage}>.",
                    caseGuid, caseStatusId, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var caseObject = JsonConvert.DeserializeObject<SagDto>(result);
            return caseObject;
        }
    }
}