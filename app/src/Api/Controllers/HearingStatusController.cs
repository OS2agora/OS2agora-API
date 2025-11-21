using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.HearingStatuses.Queries.GetHearingStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    public class HearingStatusController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<HearingStatusDto>>>> GetHearingStatus()
        {
            var operationResponse = await Mediator.Send(new GetHearingStatusesQuery());

            var hearingStatusDtoList = operationResponse.Select(hearingStatus =>
                Mapper.Map<HearingStatus, DTOs.Models.HearingStatusDto>(hearingStatus)).ToList();
            return Ok(hearingStatusDtoList);
        }
    }
}
