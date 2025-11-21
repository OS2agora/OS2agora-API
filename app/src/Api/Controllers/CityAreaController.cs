using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.CityAreas.Command.CreateCityArea;
using Agora.Operations.Models.CityAreas.Command.DeleteCityArea;
using Agora.Operations.Models.CityAreas.Command.UpdateCityArea;
using Agora.Operations.Models.CityAreas.Queries.GetCityAreas;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    public class CityAreaController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<CityAreaDto>>>> GetCityAreas()
        {
            var operationResponse = await Mediator.Send(new GetCityAreasQuery());

            var cityAreaDtoList = operationResponse.Select(cityArea => Mapper.Map<CityArea, DTOs.Models.CityAreaDto>(cityArea)).ToList();
            return Ok(cityAreaDtoList);
        }

        [HttpPost]
        public async Task<ActionResult<JsonApiTopLevelDto<CityAreaDto>>> CreateCityArea(
            JsonApiTopLevelDto<CityAreaDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.CityAreaDto, JsonApiTopLevelDto<CityAreaDto>, CityAreaDto, CityAreaDto.CityAreaAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.CityAreaDto, CityArea>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateCityAreaCommand
            {
                CityArea = resource
            });

            var cityAreaDto = Mapper.Map<CityArea, DTOs.Models.CityAreaDto>(operationResponse);
            return Ok(cityAreaDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<CityAreaDto>>> UpdateCityArea(int id,
            JsonApiTopLevelDto<CityAreaDto> dto)
        {
            var resourceDto = dto.MapFromJsonApiDtoToDto<DTOs.Models.CityAreaDto, JsonApiTopLevelDto<CityAreaDto>, CityAreaDto, CityAreaDto.CityAreaAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.CityAreaDto, CityArea>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateCityAreaCommand
            {
                CityArea = resource
            });

            var cityAreaDto = Mapper.Map<CityArea, DTOs.Models.CityAreaDto>(operationResponse);
            return Ok(cityAreaDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<CityAreaDto>>> DeleteCityArea(int id)
        {
            await Mediator.Send(new DeleteCityAreaCommand
            {
                Id = id
            });


            var cityAreaDto = new DTOs.Models.CityAreaDto
            {
                Id = id
            };
            return Ok(cityAreaDto);
        }
    }
}