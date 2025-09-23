using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ISubjectAreaDao
    {
        Task<SubjectArea> GetAsync(int id, IncludeProperties includes = null);
        Task<List<SubjectArea>> GetAllAsync(IncludeProperties includes = null);
        Task<SubjectArea> CreateAsync(SubjectArea model, IncludeProperties includes = null);
        Task<SubjectArea> UpdateAsync(SubjectArea model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}