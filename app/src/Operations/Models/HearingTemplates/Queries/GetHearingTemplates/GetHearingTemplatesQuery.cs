using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;

namespace Agora.Operations.Models.HearingTemplates.Queries.GetHearingTemplates
{
    [PostFilter("HasRole('Administrator')")]
    [PostFilter("HasAnyRole(['Anonymous', 'Citizen', 'Employee'])",
        "HearingTypes.FieldTemplates, HearingTypes.Hearings, Fields.FieldTemplates, Fields.Contents")]
    public class GetHearingTemplatesQuery : IRequest<List<HearingTemplate>>
    {
        public List<string> RequestIncludes { get; set; }

        public class GetHearingTemplatesQueryHandler : IRequestHandler<GetHearingTemplatesQuery, List<HearingTemplate>>
        {
            private readonly IHearingTemplateDao _hearingTemplateDao;

            public GetHearingTemplatesQueryHandler(IHearingTemplateDao hearingTemplateDao)
            {
                _hearingTemplateDao = hearingTemplateDao;
            }

            public async Task<List<HearingTemplate>> Handle(GetHearingTemplatesQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<HearingTemplate>(request.RequestIncludes, null);
                var hearingTemplates = await _hearingTemplateDao.GetAllAsync(includes);
                return hearingTemplates;
            }
        }
    }
}
