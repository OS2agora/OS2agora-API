using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Multiparts;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.Plugins.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly PluginConfiguration PluginConfiguration;

        protected PluginBase(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration)
        {
            ServiceProvider = serviceProvider;
            PluginConfiguration = pluginConfiguration;
        }

        public virtual Task<Hearing> BeforeHearingCreate(Hearing model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Hearing> AfterHearingCreate(Hearing model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Hearing> BeforeHearingUpdate(Hearing model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Hearing> AfterHearingUpdate(Hearing model)
        {
            throw new NotImplementedException();
        }

        public virtual Task AfterHearingDelete(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task BeforeHearingDelete(int hearingId, HearingStatus hearingStatus)
        {
            throw new NotImplementedException();
        }

        public virtual Task<UserHearingRole> AfterUserHearingRoleCreate(UserHearingRole model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<UserHearingRole> BeforeUserHearingRoleCreate(UserHearingRole model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Hearing> BeforeHearingStatusUpdate(Hearing model, HearingStatus oldStatus)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Hearing> AfterHearingStatusUpdate(Hearing model, HearingStatus oldStatus)
        {
            throw new NotImplementedException();
        }

        public virtual Task<FileOperation> BeforeFileOperation(FileOperation file)
        {
            throw new NotImplementedException();
        }

        public virtual Task<File> BeforeFileUpload(File file)
        {
            throw new NotImplementedException();
        }

        public virtual Task AfterCommentTextContentUpdate(int commentId, int contentId)
        {
            throw new NotImplementedException();
        }

        public virtual Task AfterCommentTextContentCreate(int commentId, int contentId)
        {
            throw new NotImplementedException();
        }

        public virtual Task AfterCommentFileContentCreate(int commentId, int contentId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyUsersAfterInvitedToHearing(int hearingId, List<int> userIds)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyCompaniesAfterInvitedToHearing(int hearingId, List<int> companyIds)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterHearingResponse(int hearingId, int userId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterHearingResponseDecline(int hearingId, int userId, int commentId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterConclusionPublished(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterHearingChanged(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterAddedAsReviewer(int hearingId, int userId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterChangeHearingOwner(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterChangeHearingStatus(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAfterHearingReview(int hearingId)
        {
            throw new NotImplementedException();
        }

        public virtual Task CheckSbsysKleMappings(List<KleMapping> kleMappings)
        {
            throw new NotImplementedException();
        }
    }
}