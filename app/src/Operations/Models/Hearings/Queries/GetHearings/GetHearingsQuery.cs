using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Models.Comments.Queries.GetComments;
using Agora.Operations.Common.CustomRequests;

namespace Agora.Operations.Models.Hearings.Queries.GetHearings
{
    public class GetHearingsQuery : PaginationRequest<List<Hearing>>
    {
        public List<string> RequestIncludes { get; set; }
        
        public class GetHearingsQueryHandler : IRequestHandler<GetHearingsQuery, List<Hearing>>
        {
            private readonly IHearingDao _hearingDao;
            private readonly ISender _mediator;

            public GetHearingsQueryHandler(IHearingDao hearingDao, ISender mediator)
            {
                _hearingDao = hearingDao;
                _mediator = mediator;
            }

            public async Task<List<Hearing>> Handle(GetHearingsQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(request.RequestIncludes, request.SortAndFilterIncludes);
                List<Hearing> hearings = await _hearingDao.GetAllAsync(includes);

                var comments = await _mediator.Send(new GetCommentsQuery
                {
                    HearingIds = hearings.Select(x => x.Id).ToList(),
                }, cancellationToken);
                
                foreach (Hearing hearing in hearings)
                {
                    hearing.Contents = hearing.Contents.Where(content => content.FieldId != null).ToList();
                    hearing.CommentAmount = comments.Count(c => c.HearingId == hearing.Id);
                }

                return hearings;
            }
        }
    }
}