using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IHearingStatusDao
    {
        Task<List<HearingStatus>> GetAllAsync(IncludeProperties includes = null);
        Task<HearingStatus> GetAsync(int id, IncludeProperties includes = null);
    }
}