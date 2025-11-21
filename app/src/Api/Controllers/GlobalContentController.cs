using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.GlobalContents.Commands;
using Agora.Operations.Models.GlobalContents.Queries.GetLatestGlobalContent;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    public class GlobalContentController : ApiController
    {
        [HttpGet("{globalContentTypeId}/Latest")]
        public async Task<ActionResult<JsonApiTopLevelDto<GlobalContentDto>>> GetGlobalContent(int globalContentTypeId)
        {
            var operationResponse = await Mediator.Send(new GetLatestGlobalContentQuery
            {
                GlobalContentTypeId = globalContentTypeId

            });

            var globalContentDto = Mapper.Map<GlobalContent, DTOs.Models.GlobalContentDto>(operationResponse);
            return Ok(globalContentDto);
        }

        [HttpPost("{globalContentTypeId}")]
        public async Task<ActionResult<JsonApiTopLevelDto<GlobalContentDto>>> CreateGlobalContent(int globalContentTypeId, JsonApiTopLevelDto<GlobalContentDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.GlobalContentDto, JsonApiTopLevelDto<GlobalContentDto>, GlobalContentDto, GlobalContentDto.GlobalContentAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.GlobalContentDto, GlobalContent>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateGlobalContentCommand
            {
                GlobalContent = resource,
                GlobalContentTypeId = globalContentTypeId
            });

            var dtoResult = Mapper.Map<GlobalContent, DTOs.Models.GlobalContentDto>(operationResponse);
            return Ok(dtoResult);
        }
    }
}