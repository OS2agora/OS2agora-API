using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.Plugins;
using System.Threading.Tasks;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task<UserHearingRole> AfterUserHearingRoleCreate(UserHearingRole model)
        {
            return await InvokeMethodOnPlugins(model,
                (type, accumulator) =>
                    InvokePlugin<UserHearingRole>(type, nameof(IPlugin.AfterUserHearingRoleCreate), accumulator));
        }

        public async Task<UserHearingRole> BeforeUserHearingRoleCreate(UserHearingRole model)
        {
            return await InvokeMethodOnPlugins(model,
                (type, accumulator) =>
                    InvokePlugin<UserHearingRole>(type, nameof(IPlugin.BeforeUserHearingRoleCreate), accumulator));
        }
    }
}