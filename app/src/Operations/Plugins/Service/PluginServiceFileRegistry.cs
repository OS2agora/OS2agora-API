using BallerupKommune.Models.Models.Files;
using BallerupKommune.Models.Models.Multiparts;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Plugins.Service
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