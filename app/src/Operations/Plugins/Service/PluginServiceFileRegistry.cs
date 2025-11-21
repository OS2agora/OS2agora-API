using Agora.Models.Models.Files;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Interfaces.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task<List<FileOperation>> BeforeFileOperation(IEnumerable<FileOperation> fileOperations)
        {
            var result = new List<FileOperation>();
            foreach (var fileOperation in fileOperations)
            {
                var partialResult = await InvokeMethodOnPlugins(fileOperation,
                    (type, accumulator) =>
                        InvokePlugin<FileOperation>(type, nameof(IPlugin.BeforeFileOperation), accumulator));
                result.Add(partialResult);
            }
            return result;
        }

        public async Task<File> BeforeFileUpload(File file)
        {
            return await InvokeMethodOnPlugins(file,
                    (type, accumulator) => InvokePlugin<File>(type, nameof(IPlugin.BeforeFileUpload), accumulator));
        }
    }
}