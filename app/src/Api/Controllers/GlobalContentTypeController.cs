using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.GlobalContentTypes.Queries.GetGlobalContentTypesQuery;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
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