using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Operations.Common.Interfaces.Plugins;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task NotifyUsersAfterInvitedToHearing(int hearingId, List<int> userIds)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyUsersAfterInvitedToHearing), hearingId, userIds));
        }

        public async Task NotifyCompaniesAfterInvitedToHearing(int hearingId, List<int> companyIds)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyCompaniesAfterInvitedToHearing), hearingId, companyIds));
        }

        public async Task NotifyAfterHearingResponse(int hearingId, int userId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterHearingResponse), hearingId, userId));
        }

        public async Task NotifyAfterHearingResponseDecline(int hearingId, int userId, int commentId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterHearingResponseDecline), hearingId, userId, commentId));
        }

        public async Task NotifyAfterConclusionPublished(int hearingId)
        {
            await InvokeMethodOnPlugins(type =>
                InvokePlugin(type, nameof(IPlugin.NotifyAfterConclusionPublished), hearingId));
        }

        public async Task NotifyAfterHearingChanged(int hearingId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterHearingChanged), hearingId));
        }

        public async Task NotifyAfterAddedAsReviewer(int hearingId, int userId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterAddedAsReviewer), hearingId, userId));
        }

        public async Task NotifyAfterChangeHearingOwner(int hearingId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterChangeHearingOwner), hearingId));
        }

        public async Task NotifyAfterChangeHearingStatus(int hearingId)
        {
            await InvokeMethodOnPlugins(type =>
                InvokePlugin(type, nameof(IPlugin.NotifyAfterChangeHearingStatus), hearingId));
        }

        public async Task NotifyAfterHearingReview(int hearingId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.NotifyAfterHearingReview), hearingId));
        }
    }
}