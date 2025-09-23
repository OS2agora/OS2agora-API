using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalContentType = BallerupKommune.Models.Enums.GlobalContentType;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IGlobalContentDao
    {
        Task<List<GlobalContent>> GetAllAsync(IncludeProperties includes = null);
        Task<GlobalContent> GetLatestVersionOfTypeAsync(GlobalContentType type, IncludeProperties includes = null);
        Task<GlobalContent> CreateAsync(GlobalContent model, IncludeProperties includes = null);
    }
}