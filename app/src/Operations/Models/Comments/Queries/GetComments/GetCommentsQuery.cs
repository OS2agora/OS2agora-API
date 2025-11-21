using System;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.CustomRequests;
using Agora.Operations.Common.Interfaces;
using NovaSec.Attributes;
using CommentType = Agora.Models.Enums.CommentType;

namespace Agora.Operations.Models.Comments.Queries.GetComments
{
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && @Security.IsCommentApproved(resultObject) && @Security.CanHearingShowComments(resultObject.HearingId) && !@Security.IsCommentReview(resultObject) && !@Security.IsCommentResponseReply(resultObject) && HasAnyRole(['Anonymous','Citizen','Employee']) && !resultObject.IsDeleted", 
        "CommentChildren, User.Name, User.Company.Cvr")]
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && @Security.IsCommentApproved(resultObject) && @Security.IsHearingReviewer(resultObject.HearingId) && !@Security.IsCommentResponseReply(resultObject) && !resultObject.IsDeleted",
        "CommentChildren, User.Name, User.Company.Cvr")]
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && (@Security.IsCommentOwner(resultObject) || @Security.IsCommentFromMyCompany(resultObject)) && !resultObject.IsDeleted",
        "CommentChildren, User.Name, User.Company.Cvr")]
    [PostFilter("@Security.CanSeeHearing(resultObject.HearingId) && @Security.IsHearingOwner(resultObject.HearingId) && !resultObject.IsDeleted")]
    public class GetCommentsQuery : PaginationRequest<List<Comment>>
    {
        public List<int> HearingIds { get; set; }
        public Hearing Hearing { get; set; }
        public bool MyCommentsOnly { get; set; }

        public List<string> RequestIncludes { get; set; }

        public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<Comment>>
        {
            private readonly ICommentDao _commentDao;
            private readonly ICurrentUserService _currentUserService;

            public GetCommentsQueryHandler(ICommentDao commentDao, IHearingDao hearingDao, ICurrentUserService currentUserService)
            {
                _commentDao = commentDao;
                _currentUserService = currentUserService;
            }

            public async Task<List<Comment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Comment>(request.RequestIncludes, null);
                Expression<Func<Comment, bool>> filter = null;

                var hearingIds = request.HearingIds;
                if (request.MyCommentsOnly && request.HearingIds.Any())
                {
                    var currentUserId = _currentUserService.DatabaseUserId;
                    filter = comment =>
                        comment.CommentType.Type != CommentType.HEARING_RESPONSE_REPLY &&
                        hearingIds.Contains(comment.HearingId) && currentUserId != null &&
                        comment.UserId == currentUserId;

                }
                else if (hearingIds.Any())
                {
                    filter = comment => comment.CommentType.Type != CommentType.HEARING_RESPONSE_REPLY && hearingIds.Contains(comment.HearingId);
                }
                else
                {
                    filter = comment => comment.CommentType.Type != CommentType.HEARING_RESPONSE_REPLY;
                }

                var comments = await _commentDao.GetAllAsync(includes, filter);

                return comments;
            }
        }
    }
}