using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.HearingStatuses.Queries.GetHearingStatus
{
    [PostFilter("true","Hearings")]
    public class GetHearingStatusesQuery : IRequest<List<HearingStatus>>
    {
        public List<string> Includes { get; set; }

        public class GetHearingStatusesQueryHandler : IRequestHandler<GetHearingStatusesQuery, List<HearingStatus>>
        {
            private readonly IHearingStatusDao _hearingStatusDao;

            public GetHearingStatusesQueryHandler(IHearingStatusDao hearingStatusDao)
            {
                _hearingStatusDao = hearingStatusDao;
            }

            public async Task<List<HearingStatus>> Handle(GetHearingStatusesQuery request, CancellationToken cancellationToken)
            {
                var hearingStatus = await _hearingStatusDao.GetAllAsync();
                return hearingStatus;
            }
        }
    }
}
