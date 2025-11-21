using System;
using System.Collections.Generic;
using System.Text;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using System.Threading.Tasks;

namespace Agora.DAOs.Esdh.Sbsip.V12.Interface
{
    public interface ICaseServiceV12
    {
        Task<SagDtoV10> CreateCase(int userId, string title, int templateId);

        Task<SagDtoV10> UpdateHearingOwnerOnCase(Guid caseGuid, BrugerDto userDto);

        Task<List<SagsStatusDto>> GetCaseStatus();

        Task<SagDto> CloseCase(Guid caseGuid, int caseStatusId, string comment = null);

    }
}
