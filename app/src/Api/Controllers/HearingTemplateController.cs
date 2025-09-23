using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Api.Mappings;
using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.HearingTemplates.Queries.GetHearingTemplates;
using Microsoft.AspNetCore.Mvc;

namespace BallerupKommune.Api.Controllers
{
    public class HearingTemplateController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<HearingTemplateDto>>>> GetHearingTemplates(
            [FromQuery] string include)
        {
            List<HearingTemplate> operationResponse = await Mediator.Send(new GetHearingTemplatesQuery
            {
                RequestIncludes = include.MapToIncludeList()
            });

            var hearingTemplateDtoList = operationResponse.Select(hearingTemplate =>
                Mapper.Map<HearingTemplate, DTOs.Models.HearingTemplateDto>(hearingTemplate)).ToList();
            return Ok(hearingTemplateDtoList);
        }
    }
}