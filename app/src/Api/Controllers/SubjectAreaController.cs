using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.SubjectAreas.Command.CreateSubjectArea;
using Agora.Operations.Models.SubjectAreas.Command.DeleteSubjectArea;
using Agora.Operations.Models.SubjectAreas.Command.UpdateSubjectArea;
using Agora.Operations.Models.SubjectAreas.Queries.GetSubjectAreas;
using Microsoft.AspNetCore.Mvc;

namespace Agora.Api.Controllers
{
    public class SubjectAreaController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<SubjectAreaDto>>>> GetSubjectAreas()
        {
            var operationResponse = await Mediator.Send(new GetSubjectAreasQuery());

            var subjectAreaDtoList = operationResponse.Select(subjectArea => Mapper.Map<SubjectArea, DTOs.Models.SubjectAreaDto>(subjectArea)).ToList();
            return Ok(subjectAreaDtoList);
        }

        [HttpPost]
        public async Task<ActionResult<JsonApiTopLevelDto<SubjectAreaDto>>> CreateSubjectArea(
            JsonApiTopLevelDto<SubjectAreaDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.SubjectAreaDto, JsonApiTopLevelDto<SubjectAreaDto>, SubjectAreaDto, SubjectAreaDto.SubjectAreaAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.SubjectAreaDto, SubjectArea>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateSubjectAreaCommand
            {
                SubjectArea = resource
            });

            var subjectAreaDto = Mapper.Map<SubjectArea, DTOs.Models.SubjectAreaDto>(operationResponse);
            return Ok(subjectAreaDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<SubjectAreaDto>>> UpdateSubjectArea(int id,
            JsonApiTopLevelDto<SubjectAreaDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.SubjectAreaDto, JsonApiTopLevelDto<SubjectAreaDto>, SubjectAreaDto, SubjectAreaDto.SubjectAreaAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.SubjectAreaDto, SubjectArea>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateSubjectAreaCommand
            {
                SubjectArea = resource
            });

            var subjectAreaDto = Mapper.Map<SubjectArea, DTOs.Models.SubjectAreaDto>(operationResponse);
            return Ok(subjectAreaDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<SubjectAreaDto>>> DeleteSubjectArea(int id)
        {
            await Mediator.Send(new DeleteSubjectAreaCommand
            {
                Id = id
            });

            
            var subjectAreaDto = new DTOs.Models.SubjectAreaDto
            {
                Id = id
            };
            return Ok(subjectAreaDto);
        }
    }
}