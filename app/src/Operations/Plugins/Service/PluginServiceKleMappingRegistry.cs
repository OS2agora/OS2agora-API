using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task ValidateKleMapping(List<KleMapping> kleMappings)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.CheckSbsysKleMappings), kleMappings));
        }
    }
}