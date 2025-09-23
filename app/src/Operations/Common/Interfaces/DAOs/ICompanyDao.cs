using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ICompanyDao
    {
        Task<Company> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Company>> GetAllAsync(IncludeProperties includes = null);
        Task<Company> GetCompanyByCvr(string cvr, IncludeProperties includes = null);
        Task<Company> CreateAsync(Company model, IncludeProperties includes = null);
        Task<List<Company>> CreateRangeAsync(List<Company> models, IncludeProperties includes = null);
    }
}
