using System.Collections.Generic;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IKleMappingDao
    {
        Task<KleMapping> CreateAsync(KleMapping model, IncludeProperties includes = null);
        Task<List<KleMapping>> CreateRangeAsync(List<KleMapping> models, IncludeProperties includes = null);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(int[] ids);
    }
}