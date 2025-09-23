using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.HearingTypes.Queries.GetHearingTypes
{
    [PostFilter("HasAnyRole(['Administrator', 'HearingOwner'])")]
    [PostFilter("HasAnyRole(['Anonymous', 'Citizen']) && !resultObject.IsInternalHearing", "KleMappings, HearingTemplate")]
    [PostFilter("HasRole('Employee') && resultObject.IsInternalHearing", "KleMappings, HearingTemplate")]
    public class GetHearingTypesQuery : IRequest<List<HearingType>>
    {
        public class GetHearingTypesQueryHandler : IRequestHandler<GetHearingTypesQuery, List<HearingType>>
        {
            private readonly IHearingTypeDao _hearingTypeDao;

            public GetHearingTypesQueryHandler(IHearingTypeDao hearingTypeDao)
            {
                _hearingTypeDao = hearingTypeDao;
            }

            public async Task<List<HearingType>> Handle(GetHearingTypesQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<HearingType>();
                var hearingTypes = await _hearingTypeDao.GetAllAsync(includes);
                return hearingTypes;
            }
        }
    }
}