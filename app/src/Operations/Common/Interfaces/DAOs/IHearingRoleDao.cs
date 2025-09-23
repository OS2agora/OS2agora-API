using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IHearingRoleDao
    {
        Task<HearingRole> GetAsync(int id, IncludeProperties includes = null);
        Task<List<HearingRole>> GetAllAsync(IncludeProperties includes = null);
    }
}