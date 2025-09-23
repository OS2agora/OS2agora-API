using BallerupKommune.Operations.Common.Interfaces.Plugins;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task AfterCommentTextContentUpdate(int commentId, int contentId)
        {
            await InvokeMethodOnPlugins(type =>
                InvokePlugin(type, nameof(IPlugin.AfterCommentTextContentUpdate), commentId, contentId));
        }

        public async Task AfterCommentTextContentCreate(int commentId, int contentId)
        {
            await InvokeMethodOnPlugins(type =>
                InvokePlugin(type, nameof(IPlugin.AfterCommentTextContentCreate), commentId, contentId));
        }

        public async Task AfterCommentFileContentCreate(int commentId, int contentId)
        {
            await InvokeMethodOnPlugins(type =>
                InvokePlugin(type, nameof(IPlugin.AfterCommentFileContentCreate), commentId, contentId));
        }
    }
}