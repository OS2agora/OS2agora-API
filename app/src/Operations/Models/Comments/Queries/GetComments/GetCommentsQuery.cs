using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.Comments.Queries.GetComments
{
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && @Security.IsCommentApproved(resultObject) && @Security.CanHearingShowComments(resultObject.HearingId) && !@Security.IsCommentResponseReply(resultObject) && HasAnyRole(['Anonymous','Citizen','Employee']) && !resultObject.IsDeleted")]
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && @Security.IsCommentApproved(resultObject) && @Security.IsHearingReviewer(resultObject.HearingId) && !@Security.IsCommentResponseReply(resultObject) && !resultObject.IsDeleted")]
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && (@Security.IsHearingOwner(resultObject.HearingId) ||  @Security.IsCommentOwner(resultObject) || @Security.IsCommentFromMyCompany(resultObject)) && !resultObject.IsDeleted")]
    public class GetCommentsQuery : IRequest<List<Comment>>
    {
        public List<int> HearingIds { get; set; }
        public Hearing Hearing { get; set; }

        public List<string> RequestIncludes { get; set; }

        public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<Comment>>
        {
            private readonly ICommentDao _commentDao;

            public GetCommentsQueryHandler(ICommentDao commentDao, IHearingDao hearingDao)
            {
                _commentDao = commentDao;
            }

            public async Task<List<Comment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Comment>(request.RequestIncludes, null);
                var comments = await _commentDao.GetAllAsync(includes, request.HearingIds);

                return comments;
            }
        }
    }
}