using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IKleService
    {
        Task<List<KleHierarchy>> GetKleHierarchies();
    }
}