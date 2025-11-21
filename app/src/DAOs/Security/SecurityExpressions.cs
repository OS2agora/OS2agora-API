using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Security;
using Agora.Operations.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using CommentStatus = Agora.Models.Enums.CommentStatus;
using CommentType = Agora.Models.Enums.CommentType;
using HearingRole = Agora.Models.Enums.HearingRole;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.DAOs.Security
{
    public class SecurityExpressions : ISecurityExpressions
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserDao _userDao;
        private readonly IHearingDao _hearingDao;
        private readonly IHearingRoleDao _hearingRoleDao;
        private readonly ICommentDao _commentDao;
        private readonly IHearingAccessResolver _hearingAccessResolver;
        private readonly IUserHearingRoleResolver _userHearingRoleResolver;
        private readonly ICompanyHearingRoleResolver _companyHearingRoleResolver;
        private readonly IOptions<SecurityOptions> _securityOptions;

        public SecurityExpressions(ICurrentUserService currentUserService, IUserDao userDao, IHearingDao hearingDao,
            IHearingRoleDao hearingRoleDao, ICommentDao commentDao, IHearingAccessResolver hearingAccessResolver, 
            IUserHearingRoleResolver userHearingRoleResolver, ICompanyHearingRoleResolver companyHearingRoleResolver, IOptions<SecurityOptions> securityOptions)
        {
            _currentUserService = currentUserService;
            _userDao = userDao;
            _hearingDao = hearingDao;
            _hearingRoleDao = hearingRoleDao;
            _commentDao = commentDao;
            _hearingAccessResolver = hearingAccessResolver;
            _userHearingRoleResolver = userHearingRoleResolver;
            _companyHearingRoleResolver = companyHearingRoleResolver;
            _securityOptions = securityOptions;
        }

        public bool IsCurrentUser(int id)
        {
            var userIdentifier = _currentUserService.UserId;
            var currentUser = _userDao.FindUserByIdentifier(userIdentifier).GetAwaiter().GetResult();
            return currentUser != null && currentUser.Id == id;
        }

        public bool IsHearingOwner(int hearingId) 
        {
            return _userHearingRoleResolver.IsHearingOwner(hearingId).GetAwaiter().GetResult();
        }

        public bool IsHearingOwnerByHearingId(int hearingId)
        {
            return IsCurrentUserInRoleOnHearingByHearingId(hearingId, HearingRole.HEARING_OWNER);
        }

        public bool IsHearingReviewer(int hearingId)
        {
            return _userHearingRoleResolver.IsHearingReviewer(hearingId).GetAwaiter().GetResult();
        }

        public bool IsHearingReviewerByHearingId(int hearingId)
        {
            return IsCurrentUserInRoleOnHearingByHearingId(hearingId, HearingRole.HEARING_REVIEWER);
        }

        public bool IsHearingInvitee(Hearing hearing)
        {
            return IsCurrentUserInRoleOnHearing(hearing, HearingRole.HEARING_INVITEE);
        }

        public bool IsHearingResponder(int hearingId)
        {
            return IsCurrentUserInRoleOnHearingByHearingId(hearingId, HearingRole.HEARING_RESPONDER);
        }
        
        public bool HasRoleOnAnyHearing(HearingRole role)
        {
            return _userHearingRoleResolver.UserHearingRoleExists(null, role).Result;
        }

        public bool IsCommentOwnerByCommentId(int commentId)
        {
            var includes = IncludeProperties.Create<Comment>(null, new List<string> { nameof(Comment.User) });
            var comment = _commentDao.GetAsync(commentId, includes).GetAwaiter().GetResult();

            return IsCommentOwner(comment);
        }

        public bool IsCommentOwner(Comment comment)
        {
            if (comment == null) return false;

            var currentUserId = _currentUserService.DatabaseUserId;

            if (comment.User == null)
            {
                var includes = IncludeProperties.Create<Comment>(null, new List<string> { nameof(Comment.User) });
                comment = _commentDao.GetAsync(comment.Id, includes).GetAwaiter().GetResult();
            }

            return comment.User.Id == currentUserId;
        }

        public bool IsCommentFromMyCompany(Comment comment)
        {
            if (comment == null) return false;
            var currentUserId = _currentUserService.DatabaseUserId;
            if (currentUserId == null) return false;
            var companyId = _currentUserService.CompanyId;
            if (companyId == null) return false;

            if (comment.User == null)
            {
                var includes = IncludeProperties.Create<Comment>(null, new List<string> { nameof(Comment.User) });
                comment = _commentDao.GetAsync(comment.Id, includes).GetAwaiter().GetResult();
            }
            return comment.User.CompanyId == companyId;
        }

        public bool CanSeeHearing(int hearingId)
        {
            bool canSeeHearing = _hearingAccessResolver.CanSeeHearingById(hearingId).GetAwaiter().GetResult();
            return canSeeHearing;
        }

        public bool IsCommentApproved(Comment comment)
        {
            // ensure that comment has required includes
            if (comment.CommentType == null)
            {
                var missingIncludes = new List<string>
                {
                    comment.CommentType == null ? nameof(comment.CommentType) : null,
                    comment.CommentStatus == null ? nameof(comment.CommentStatus) : null
                };

                throw new ArgumentException(
                    $"Comment is missing required include(s): {string.Join(", ", missingIncludes.Where(x => x != null))}");
            }

            return comment.CommentType.Type == CommentType.HEARING_REVIEW ||
                   comment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY ||
                   comment.CommentStatus?.Status == CommentStatus.APPROVED;
        }

        public bool IsCommentReview(Comment comment)
        {
            // ensure that comment has required includes
            if (comment.CommentType == null)
            {
                var missingIncludes = new List<string>
                {
                    comment.CommentType == null ? nameof(comment.CommentType) : null,
                };

                throw new ArgumentException(
                    $"Comment is missing required include(s): {string.Join(", ", missingIncludes.Where(x => x != null))}");
            }

            return comment.CommentType.Type == CommentType.HEARING_REVIEW;
        }

        public bool IsCommentResponseReply(Comment comment)
        {
            // ensure that comment has required include
            if (comment.CommentType == null)
            {
                throw new ArgumentException($"Comment is missing required include: {nameof(comment.CommentType)}");
            }

            return comment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY;
        }

        public bool CanHearingShowComments(int hearingId)
        {
            return _hearingAccessResolver.CanHearingShowComments(hearingId).GetAwaiter().GetResult();
        }

        private bool IsCurrentUserInRoleOnHearingByHearingId(int hearingId, HearingRole role)
        {
            var defaultIncludes = IncludeProperties.Create<Hearing>();
            var hearing = _hearingDao.GetAsync(hearingId, defaultIncludes).GetAwaiter().GetResult();

            return IsCurrentUserInRoleOnHearing(hearing, role);
        }

        private bool IsCurrentUserInRoleOnHearing(Hearing hearing, HearingRole role)
        {
            var userHasRole = _userHearingRoleResolver.UserHearingRoleExists(hearing.Id, role).GetAwaiter().GetResult();
            var companyHasRole = _companyHearingRoleResolver.CompanyHearingRoleExist(hearing.Id, role).GetAwaiter()
                .GetResult();

            return userHasRole || companyHasRole;
        }

        public bool IsHearingPublished(Hearing hearing)
        {
            if (hearing is { HearingStatus: null })
            {
                throw new ArgumentException(
                    $"Hearing is missing required include: {nameof(hearing.HearingStatus)}");
            }

            if (!_securityOptions.Value.IncludeAwaitingStartdate)
            {
                return hearing != null &&
                       hearing.HearingStatus.Status != HearingStatus.CREATED &&
                       hearing.HearingStatus.Status != HearingStatus.DRAFT &&
                       hearing.HearingStatus.Status != HearingStatus.AWAITING_STARTDATE;
            }

            return hearing != null && 
                   hearing.HearingStatus.Status != HearingStatus.CREATED &&
                   hearing.HearingStatus.Status != HearingStatus.DRAFT;
        }

        public bool IsHearingOwnerRole(int hearingRoleId)
        {
            var hearingRole = _hearingRoleDao.GetAsync(hearingRoleId).GetAwaiter().GetResult();
            return hearingRole != null && hearingRole.Role == HearingRole.HEARING_OWNER;
        }

        public bool IsInternalHearing(Hearing hearing)
        {
            // no check for HearingType include - can be NULL for a hearing
            return hearing?.HearingType?.IsInternalHearing ?? true;
        }

        public bool CanSeeSubjectArea(SubjectArea subjectArea)
        {
            bool canSeeSubjectArea = _hearingAccessResolver.CanSeeHearingBySubjectAreaId(subjectArea.Id).GetAwaiter().GetResult();
            return canSeeSubjectArea;
        }

        public bool CanSeeCityArea(CityArea cityArea)
        {
            bool canSeeCityArea = _hearingAccessResolver.CanSeeHearingByCityAreaId(cityArea.Id).GetAwaiter().GetResult();
            return canSeeCityArea;
        }
    }
}