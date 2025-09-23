using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IContentTypeDao
    {
        Task<List<ContentType>> GetAllAsync(IncludeProperties includes = null);
    }
}