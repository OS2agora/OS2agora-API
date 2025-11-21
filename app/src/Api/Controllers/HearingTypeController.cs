using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.FieldTemplates.Commands.CreateFieldTemplate;
using Agora.Operations.Models.FieldTemplates.Commands.DeleteFieldTemplate;
using Agora.Operations.Models.FieldTemplates.Commands.UpdateFieldTemplate;
using Agora.Operations.Models.FieldTemplates.Queries.GetFieldTemplates;
using Agora.Operations.Models.HearingTypes.Commands.CreateHearingType;
using Agora.Operations.Models.HearingTypes.Commands.DeleteHearingType;
using Agora.Operations.Models.HearingTypes.Commands.UpdateHearingType;
using Agora.Operations.Models.HearingTypes.Queries.GetHearingTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Operations.Models.InvitationGroupMappings.Commands.UpdateInvitationGroupMappings;
using Agora.Operations.Models.KleMappings.Commands.UpdateKleMappings;

namespace Agora.Api.Controllers
{
    [Authorize]
    public class HearingTypeController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<JsonApiTopLevelDto<List<HearingTypeDto>>>> GetHearingTypes()
        {
            var operationResponse = await Mediator.Send(new GetHearingTypesQuery());

            var hearingTypeDtoList = operationResponse
                .Select(hearingType => Mapper.Map<HearingType, DTOs.Models.HearingTypeDto>(hearingType)).ToList();
            return Ok(hearingTypeDtoList);
        }

        [HttpPost]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingTypeDto>>> CreateHearingType(
            JsonApiTopLevelDto<HearingTypeDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.HearingTypeDto, JsonApiTopLevelDto<HearingTypeDto>, HearingTypeDto, HearingTypeDto.HearingTypeAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.HearingTypeDto, HearingType>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateHearingTypeCommand
            {
                HearingType = resource
            });

            var hearingTypeDto = Mapper.Map<HearingType, DTOs.Models.HearingTypeDto>(operationResponse);
            return Ok(hearingTypeDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingTypeDto>>> UpdateHearingType(int id,
            JsonApiTopLevelDto<HearingTypeDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.HearingTypeDto, JsonApiTopLevelDto<HearingTypeDto>, HearingTypeDto, HearingTypeDto.HearingTypeAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.HearingTypeDto, HearingType>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateHearingTypeCommand
            {
                HearingType = resource
            });

            var hearingTypeDto = Mapper.Map<HearingType, DTOs.Models.HearingTypeDto>(operationResponse);
            return Ok(hearingTypeDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingTypeDto>>> DeleteHearingType(int id)
        {
            await Mediator.Send(new DeleteHearingTypeCommand
            {
                Id = id
            });


            var hearingTypeDto = new DTOs.Models.HearingTypeDto
            {
                Id = id
            };
            return Ok(hearingTypeDto);
        }

        [HttpGet("{hearingTypeId}/FieldTemplate")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<FieldTemplateDto>>>> GetFieldTemplates(int hearingTypeId)
        {
            var operationResponse = await Mediator.Send(new GetFieldTemplatesQuery());

            var fieldTemplatesDtoList = operationResponse
                .Select(fieldTemplate => Mapper.Map<FieldTemplate, DTOs.Models.FieldTemplateDto>(fieldTemplate)).ToList();
            return Ok(fieldTemplatesDtoList);
        }

        [HttpPost("{hearingTypeId}/FieldTemplate")]
        public async Task<ActionResult<JsonApiTopLevelDto<FieldTemplateDto>>> CreateFieldTemplate(int hearingTypeId,
            JsonApiTopLevelDto<FieldTemplateDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.FieldTemplateDto, JsonApiTopLevelDto<FieldTemplateDto>, FieldTemplateDto, FieldTemplateDto.FieldTemplateAttributeDto>();

            if (resourceDto.HearingType.Id != hearingTypeId)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.FieldTemplateDto, FieldTemplate>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateFieldTemplateCommand
            {
                FieldTemplate = resource
            });

            var fieldTemplateDto = Mapper.Map<FieldTemplate, DTOs.Models.FieldTemplateDto>(operationResponse);
            return Ok(fieldTemplateDto);
        }

        [HttpPatch("{hearingTypeId}/FieldTemplate/{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<FieldTemplateDto>>> UpdateFieldTemplate(int hearingTypeId,
            int id, JsonApiTopLevelDto<FieldTemplateDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.FieldTemplateDto, JsonApiTopLevelDto<FieldTemplateDto>, FieldTemplateDto, FieldTemplateDto.FieldTemplateAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.FieldTemplateDto, FieldTemplate>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateFieldTemplateCommand
            {
                FieldTemplate = resource
            });

            var fieldTemplateDto = Mapper.Map<FieldTemplate, DTOs.Models.FieldTemplateDto>(operationResponse);
            return Ok(fieldTemplateDto);
        }

        [HttpDelete("{hearingTypeId}/FieldTemplate/{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<FieldTemplateDto>>> DeleteFieldTemplate(int hearingTypeId,
            int id)
        {
            await Mediator.Send(new DeleteFieldTemplateCommand
            {
                Id = id,
                HearingTypeId = hearingTypeId
            });

            var fieldTemplateDto = new DTOs.Models.FieldTemplateDto
            {
                Id = id
            };
            return Ok(fieldTemplateDto);
        }

        [HttpPatch("{hearingTypeId}/KleMappings")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<KleMappingDto>>>> UpdateKleMappings(int hearingTypeId,
            List<JsonApiTopLevelDto<KleMappingDto>> dtos)
        {
            var resourceDtos = dtos.Select(dto =>
                dto.MapFromJsonApiDtoToDto<DTOs.Models.KleMappingDto, JsonApiTopLevelDto<KleMappingDto>, KleMappingDto,
                    KleMappingDto.KleMappingAttributeDto>());

            var resources = resourceDtos
                .Select(resourceDto => Mapper.Map<DTOs.Models.KleMappingDto, KleMapping>(resourceDto)).ToList();

            if (resources.Any(resource => resource?.HearingTypeId != hearingTypeId))
            {
                return BadRequest("HearingType Id from DTO does not match URL parameter");
            }

            var operationResponse = await Mediator.Send(new UpdateKleMappingsCommand
            {
                HearingTypeId = hearingTypeId,
                KleMappings = resources
            });

            var kleMappingDtoList = operationResponse.Select(kleMapping => Mapper.Map<KleMapping, DTOs.Models.KleMappingDto>(kleMapping)).ToList();
            return Ok(kleMappingDtoList);
        }

        [HttpPatch("{hearingTypeId}/InvitationGroupMappings")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<InvitationGroupMappingDto>>>> UpdateInvitationGroupMappings(int hearingTypeId, List<JsonApiTopLevelDto<InvitationGroupMappingDto>> dtos)
        {
            var resourceDtos = dtos.Select(dto => dto.MapFromJsonApiDtoToDto<DTOs.Models.InvitationGroupMappingDto, JsonApiTopLevelDto<InvitationGroupMappingDto>, InvitationGroupMappingDto, InvitationGroupMappingDto.InvitationGroupMappingAttributeDto>());

            var resources = resourceDtos.Select(resourceDto => Mapper.Map<DTOs.Models.InvitationGroupMappingDto, InvitationGroupMapping>(resourceDto)).ToList();

            if (resources.Any(resource => resource?.HearingTypeId != hearingTypeId))
            {
                return BadRequest("HearingType Id from DTO does not match URL parameter");
            }

            var operationResponse = await Mediator.Send(new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = hearingTypeId,
                InvitationGroupMappings = resources
            });

            var invitationGroupMappingDtoList = operationResponse.Select(invitationGroupMapping => Mapper.Map<InvitationGroupMapping, DTOs.Models.InvitationGroupMappingDto>(invitationGroupMapping)).ToList();
            return Ok(invitationGroupMappingDtoList);
        }
    }
}