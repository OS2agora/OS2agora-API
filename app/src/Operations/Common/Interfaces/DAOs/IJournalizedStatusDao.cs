using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IJournalizeStatusDao
    {
        Task<List<JournalizedStatus>> GetAllAsync(IncludeProperties includes = null);
    }
}
