using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IFieldDao
    {
        Task<Field> GetAsync(int id, IncludeProperties includes = null);
    }
}