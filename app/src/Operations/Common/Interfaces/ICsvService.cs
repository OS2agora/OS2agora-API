using BallerupKommune.Models.Models.Files;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface ICsvService
    {
        Task<List<T>> ParseCsv<T>(File file) where T : class;
    }
}