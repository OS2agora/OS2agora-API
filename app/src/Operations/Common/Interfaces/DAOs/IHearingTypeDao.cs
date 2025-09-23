using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IHearingTypeDao
    {
        Task<HearingType> GetAsync(int id, IncludeProperties includes = null);
        Task<List<HearingType>> GetAllAsync(IncludeProperties includes = null);
        Task<HearingType> CreateAsync(HearingType model, IncludeProperties includes = null);
        Task<HearingType> UpdateAsync(HearingType model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}
