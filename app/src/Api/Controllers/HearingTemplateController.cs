using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.Models.Models;
using Agora.Operations.Models.HearingTemplates.Queries.GetHearingTemplates;
using Microsoft.AspNetCore.Mvc;

namespace Agora.Api.Controllers
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