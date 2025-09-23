using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.HearingRoles.Queries.GetHearingRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
{
    [Authorize]
    public class HearingRoleController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<HearingRoleDto>>>> GetHearingRoles()
        {
            var operationResponse = await Mediator.Send(new GetHearingRolesQuery());

            var hearingRoleDtoList = operationResponse.Select(hearingRole => Mapper.Map<HearingRole, DTOs.Models.HearingRoleDto>(hearingRole)).ToList();
            return Ok(hearingRoleDtoList);
        }
    }
}