using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.InvitationSources.Queries.GetInvitationSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    [Authorize]
    public class InvitationSourceController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<InvitationSourceDto>>>> GetInvitationSources()
        {
            var operationResponse = await Mediator.Send(new GetInvitationSourcesQuery());

            var invitationSourceDtoList = operationResponse.Select(invitationSource => Mapper.Map<InvitationSource, DTOs.Models.InvitationSourceDto>(invitationSource)).ToList();
            return Ok(invitationSourceDtoList);
        }
    }
}
