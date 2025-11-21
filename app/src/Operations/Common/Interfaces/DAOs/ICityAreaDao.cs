using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface ICityAreaDao
    {
        Task<CityArea> GetAsync(int id, IncludeProperties includes = null);
        Task<List<CityArea>> GetAllAsync(IncludeProperties includes = null);
        Task<CityArea> CreateAsync(CityArea model, IncludeProperties includes = null);
        Task<CityArea> UpdateAsync(CityArea model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}