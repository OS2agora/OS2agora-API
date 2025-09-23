using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.GlobalContentTypes.Queries.GetGlobalContentTypesQuery;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
{
    public class GlobalContentTypeController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<GlobalContentTypeDto>>>> GetGlobalContentTypes()
        {
            var operationResponse = await Mediator.Send(new GetGlobalContentTypesQuery());

            var globalContentTypeDtoList = operationResponse.Select(globalContentType =>
                Mapper.Map<GlobalContentType, DTOs.Models.GlobalContentTypeDto>(globalContentType)).ToList();
            return Ok(globalContentTypeDtoList);
        }
    }
}