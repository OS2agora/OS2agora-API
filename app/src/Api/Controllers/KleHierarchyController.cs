using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.KleHierarchies.Queries.GetKleHierarchies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
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