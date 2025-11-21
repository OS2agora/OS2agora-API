using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.KleHierarchies.Queries.GetKleHierarchies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Api.Controllers
{
    [Authorize]
    public class KleHierarchyController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<KleHierarchyDto>>>> GetKleHierarchies()
        {
            var operationResponse = await Mediator.Send(new GetKleHierarchiesQuery());

            var kleHierarchyDtoList = operationResponse.Select(kleHierarchy => Mapper.Map<KleHierarchy, DTOs.Models.KleHierarchyDto>(kleHierarchy)).ToList();
            return Ok(kleHierarchyDtoList);
        }
    }
}