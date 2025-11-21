using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
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
