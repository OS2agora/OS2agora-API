using System.Collections.Generic;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Exceptions;

namespace BallerupKommune.Operations.Models.Hearings.Queries.GetHearing
{
    public class GetHearingQuery : IRequest<Hearing>
    {
        public int Id { get; set; }
        public List<string> RequestIncludes { get; set; }

        public class GetHearingQueryHandler : IRequestHandler<GetHearingQuery, Hearing>
        {
            private readonly IHearingDao _hearingDao;

            public GetHearingQueryHandler(IHearingDao hearingDao)
            {
                _hearingDao = hearingDao;
            }

            public async Task<Hearing> Handle(GetHearingQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(request.RequestIncludes, null);
                var hearing = await _hearingDao.GetAsync(request.Id, includes);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Id);
                }

                return hearing;
            }
        }
    }
}