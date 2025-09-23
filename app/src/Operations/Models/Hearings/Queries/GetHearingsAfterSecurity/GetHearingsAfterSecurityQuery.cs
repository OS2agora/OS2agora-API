using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.Hearings.Queries.GetHearingsAfterSecurity
{
    public class GetHearingAccessIdsQuery : IRequest<List<Hearing>>
    {
        public class GetHearingIdsQueryHandler : IRequestHandler<GetHearingAccessIdsQuery, List<Hearing>>
        {
            private readonly IHearingDao _hearingDao;

            public GetHearingIdsQueryHandler(IHearingDao hearingDao)
            {
                _hearingDao = hearingDao;
            }

            public async Task<List<Hearing>> Handle(GetHearingAccessIdsQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>();
                List<Hearing> hearings = await _hearingDao.GetAllAsync(includes);
                return hearings;
            }
        }
    }
}