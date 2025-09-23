using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IHearingStatusDao
    {
        Task<List<HearingStatus>> GetAllAsync(IncludeProperties includes = null);
        Task<HearingStatus> GetAsync(int id, IncludeProperties includes = null);
    }
}