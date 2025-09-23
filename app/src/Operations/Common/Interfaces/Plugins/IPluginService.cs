using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Multiparts;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Models.Files;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Common.Interfaces.Plugins
{
    public interface IPluginService
    {
        // HearingRegistry
        Task<Hearing> BeforeHearingCreate(Hearing model);
        Task<Hearing> AfterHearingCreate(Hearing model);

        Task<Hearing> BeforeHearingUpdate(Hearing model, HearingStatus oldStatus);
        Task<Hearing> AfterHearingUpdate(Hearing model, HearingStatus oldStatus);
        Task BeforeHearingDelete(int hearingId, HearingStatus hearingStatus);
        Task AfterHearingDelete(int hearingId);


        // UserHearingRoleRegistry
        Task<UserHearingRole> AfterUserHearingRoleCreate(UserHearingRole model);
        Task<UserHearingRole> BeforeUserHearingRoleCreate(UserHearingRole model);


        // FileRegistry
        Task<List<FileOperation>> BeforeFileOperation(IEnumerable<FileOperation> files);
        Task<File> BeforeFileUpload(File file);


        // CommentRegistry
        Task AfterCommentTextContentUpdate(int commentId, int contentId);
        Task AfterCommentTextContentCreate(int commentId, int contentId);
        Task AfterCommentFileContentCreate(int commentId, int contentId);


        // NotificationRegistry
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


        // SbsysKleMappingRegistry
        Task ValidateKleMapping(List<KleMapping> kleMappings);
    }
} 