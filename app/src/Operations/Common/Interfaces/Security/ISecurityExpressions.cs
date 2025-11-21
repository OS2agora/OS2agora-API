using Agora.Models.Models;
using HearingRole = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.Common.Interfaces.Security
{
    public interface ISecurityExpressions
    {
        bool IsCurrentUser(int id);
        bool IsHearingOwner(int hearingId);
        bool IsHearingOwnerByHearingId(int hearingId);
        bool IsHearingOwnerRole(int hearingRoleId);
        bool IsHearingReviewer(int hearingId);
        bool IsHearingReviewerByHearingId(int hearingId);
        bool IsHearingInvitee(Hearing hearing);
        bool HasRoleOnAnyHearing(HearingRole role);
        bool IsHearingPublished(Hearing hearing);
        bool IsInternalHearing(Hearing hearing);
        public bool CanSeeSubjectArea(SubjectArea subjectArea);
        public bool CanSeeCityArea(CityArea cityArea);
        bool IsCommentOwner(Comment comment);
        bool IsCommentFromMyCompany(Comment comment);
        bool IsCommentOwnerByCommentId(int commentId);
        bool IsHearingResponder(int hearingId);
        bool CanSeeHearing(int hearingId);
        bool IsCommentApproved(Comment comment);
        bool IsCommentResponseReply(Comment comment);
        public bool CanHearingShowComments(int hearingId);
        bool IsCommentReview(Comment comment);
    }
}