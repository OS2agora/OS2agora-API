using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.HearingStatuses.Queries.GetHearingStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
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
