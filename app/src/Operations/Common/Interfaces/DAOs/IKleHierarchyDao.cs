using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IKleHierarchyDao
    {
        Task<KleHierarchy> GetAsync(int id, IncludeProperties includes = null);
        Task<List<KleHierarchy>> GetAllAsync(IncludeProperties includes = null);
    }
}