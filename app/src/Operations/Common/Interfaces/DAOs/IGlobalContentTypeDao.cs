using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IGlobalContentTypeDao
    {
        Task<List<GlobalContentType>> GetAllAsync(IncludeProperties includes = null);
        Task<GlobalContentType> GetAsync(int id, IncludeProperties includes = null);
    }
}