using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalContentType = Agora.Models.Enums.GlobalContentType;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IGlobalContentDao
    {
        Task<List<GlobalContent>> GetAllAsync(IncludeProperties includes = null);
        Task<GlobalContent> GetLatestVersionOfTypeAsync(GlobalContentType type, IncludeProperties includes = null);
        Task<GlobalContent> CreateAsync(GlobalContent model, IncludeProperties includes = null);
    }
}