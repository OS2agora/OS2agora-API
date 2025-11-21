using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces
{
    public interface IKleService
    {
        Task<List<KleHierarchy>> GetKleHierarchies();
    }
}