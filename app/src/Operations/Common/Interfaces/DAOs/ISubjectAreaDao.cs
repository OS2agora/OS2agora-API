using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
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