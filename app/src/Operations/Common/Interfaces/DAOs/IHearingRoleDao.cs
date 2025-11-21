using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IHearingRoleDao
    {
        Task<HearingRole> GetAsync(int id, IncludeProperties includes = null);
        Task<List<HearingRole>> GetAllAsync(IncludeProperties includes = null);
    }
}