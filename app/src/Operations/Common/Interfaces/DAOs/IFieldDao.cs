using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IFieldDao
    {
        Task<Field> GetAsync(int id, IncludeProperties includes = null);
    }
}