using Agora.Models.Models.Files;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Files
{
    public interface ICsvService
    {
        Task<List<T>> ParseCsv<T>(File file, Dictionary<string, string> columnMappings = null) where T : class;
    }
}