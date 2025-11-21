using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IKleHierarchyDao
    {
        Task<KleHierarchy> GetAsync(int id, IncludeProperties includes = null);
        Task<List<KleHierarchy>> GetAllAsync(IncludeProperties includes = null);
    }
}