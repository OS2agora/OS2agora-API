using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.HearingRoles.Queries.GetHearingRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
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