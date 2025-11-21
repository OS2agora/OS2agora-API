using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.InvitationGroups.Commands.CreateInvitationGroup;
using Agora.Operations.Models.InvitationGroups.Commands.DeleteInvitationGroup;
using Agora.Operations.Models.InvitationGroups.Commands.UpdateInvitationGroup;
using Agora.Operations.Models.InvitationGroups.Queries.GetInvitationGroups;
using Agora.Operations.Models.InvitationKeys.Commands.UpdateInvitationKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    [Authorize]
    public class InvitationGroupController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<InvitationGroupDto>>>> GetInvitationGroups()
        {
            var operationResponse = await Mediator.Send(new GetInvitationGroupsQuery());

            var invitationGroupDtoList = operationResponse.Select(invitationGroup => Mapper.Map<InvitationGroup, DTOs.Models.InvitationGroupDto>(invitationGroup)).ToList();
            return Ok(invitationGroupDtoList);
        }

        [HttpPost]
        public async Task<ActionResult<JsonApiTopLevelDto<InvitationGroupDto>>> CreateInvitationGroup(JsonApiTopLevelDto<InvitationGroupDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.InvitationGroupDto, JsonApiTopLevelDto<InvitationGroupDto>, InvitationGroupDto, InvitationGroupDto.InvitationGroupAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.InvitationGroupDto, InvitationGroup>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateInvitationGroupCommand
            {
                InvitationGroup = resource
            });

            var invitationGroupDto = Mapper.Map<InvitationGroup, DTOs.Models.InvitationGroupDto>(operationResponse);
            return Ok(invitationGroupDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<InvitationGroupDto>>> UpdateInvitationGroup(int id, JsonApiTopLevelDto<InvitationGroupDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.InvitationGroupDto, JsonApiTopLevelDto<InvitationGroupDto>, InvitationGroupDto, InvitationGroupDto.InvitationGroupAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.InvitationGroupDto, InvitationGroup>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateInvitationGroupCommand
            {
                InvitationGroup = resource
            });

            var invitationGroupDto = Mapper.Map<InvitationGroup, DTOs.Models.InvitationGroupDto>(operationResponse);
            return Ok(invitationGroupDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<InvitationGroupDto>>> DeleteInvitationGroup(int id)
        {
            await Mediator.Send(new DeleteInvitationGroupCommand
            {
                Id = id
            });

            var invitationGroupDto = new DTOs.Models.InvitationGroupDto
            {
                Id = id
            };
            return Ok(invitationGroupDto);
        }

        [HttpPatch("{invitationGroupId}/InvitationKeys")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<InvitationKeyDto>>>> UpdateInvitationGroupMappings(int invitationGroupId, List<JsonApiTopLevelDto<InvitationKeyDto>> dtos)
        {
            var resourceDtos = dtos.Select(dto => dto.MapFromJsonApiDtoToDto<DTOs.Models.InvitationKeyDto, JsonApiTopLevelDto<InvitationKeyDto>, InvitationKeyDto, InvitationKeyDto.InvitationKeyAttributeDto>());

            var resources = resourceDtos.Select(resourceDto => Mapper.Map<DTOs.Models.InvitationKeyDto, InvitationKey>(resourceDto)).ToList();

            if (resources.Any(resource => resource?.InvitationGroupId != invitationGroupId))
            {
                return BadRequest("InvitationGroup Id from DTO does not match URL parameter");
            }

            var operationResponse = await Mediator.Send(new UpdateInvitationKeysCommand
            {
                InvitationGroupId = invitationGroupId,
                InvitationKeys = resources
            });

            var invitationKeyDtoList = operationResponse.Select(invitationKey => Mapper.Map<InvitationKey, DTOs.Models.InvitationKeyDto>(invitationKey)).ToList();
            return Ok(invitationKeyDtoList);
        }
    }
}

