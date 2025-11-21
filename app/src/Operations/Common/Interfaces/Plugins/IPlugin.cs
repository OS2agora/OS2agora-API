using System.Collections.Generic;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Multiparts;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.Common.Interfaces.Plugins
{
    public interface IPlugin
    {
        // Hearings related plugins
        Task<Hearing> BeforeHearingCreate(Hearing model);
        Task<Hearing> AfterHearingCreate(Hearing model);

        Task<Hearing> BeforeHearingUpdate(Hearing model);
        Task<Hearing> AfterHearingUpdate(Hearing model);

        Task<Hearing> BeforeHearingStatusUpdate(Hearing model, HearingStatus oldStatus);
        Task<Hearing> AfterHearingStatusUpdate(Hearing model, HearingStatus oldStatus);

        Task BeforeHearingDelete(int hearingId, HearingStatus hearingStatus);
        Task AfterHearingDelete(int hearingId);


        // UserHearingRoles related plugins
        Task<UserHearingRole> AfterUserHearingRoleCreate(UserHearingRole model);
        Task<UserHearingRole> BeforeUserHearingRoleCreate(UserHearingRole model);


        // File related plugins
        Task<FileOperation> BeforeFileOperation(FileOperation file);
        Task<File> BeforeFileUpload(File file);


        // Comment related plugins
        Task AfterCommentTextContentUpdate(int commentId, int contentId);
        Task AfterCommentTextContentCreate(int commentId, int contentId);
        Task AfterCommentFileContentCreate(int commentId, int contentId);


        // Notification related plugins
        Task NotifyUsersAfterInvitedToHearing(int hearingId, List<int> userIds);
        Task NotifyCompaniesAfterInvitedToHearing(int hearingId, List<int> companyIds);
        Task NotifyAfterHearingResponse(int hearingId, int userId);
        Task NotifyAfterHearingResponseDecline(int hearingId, int userId, int commentId);
        Task NotifyAfterConclusionPublished(int hearingId);
        Task NotifyAfterHearingChanged(int hearingId);

        Task NotifyAfterAddedAsReviewer(int hearingId, int userId);
        Task NotifyAfterChangeHearingOwner(int hearingId);
        Task NotifyAfterChangeHearingStatus(int hearingId);
        Task NotifyAfterHearingReview(int hearingId);


        // SbsysKleMapping related plugins
        Task CheckSbsysKleMappings(List<KleMapping> kleMappings);
    }
}