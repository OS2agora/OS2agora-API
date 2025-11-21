using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IGlobalContentTypeDao
    {
        Task<List<GlobalContentType>> GetAllAsync(IncludeProperties includes = null);
        Task<GlobalContentType> GetAsync(int id, IncludeProperties includes = null);
    }
}