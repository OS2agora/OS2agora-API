using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.DAOs.Esdh.Sbsip.V12.Mock
{
    public class CaseServiceV12Mock : ICaseServiceV12
    {
        public async Task<SagDtoV10> CreateCase(int userId, string title, int templateId)
        {
            SagDtoV10 newcase = new SagDtoV10
            {
                Id = userId,
                SagIdentity = Guid.NewGuid(),
                Nummer = title
            };
            return newcase;
        }

        public async Task<SagDtoV10> UpdateHearingOwnerOnCase(Guid caseGuid, BrugerDto userDto)
        {
            var caseObject = new SagDtoV10 { Behandler = userDto };

            return caseObject;
        }

        public async Task<List<SagsStatusDto>> GetCaseStatus()
        {
            var singlecase = new SagsStatusDto { Navn = "Afsluttet" };
            var caseStatus = new List<SagsStatusDto> { singlecase };
            return caseStatus;
        }

        public async Task<SagDto> CloseCase(Guid caseGuid, int caseStatusId, string comment = null)
        {

            var caseObject = new SagDto { SagIdentity = caseGuid };
            return caseObject;
        }
    }
}