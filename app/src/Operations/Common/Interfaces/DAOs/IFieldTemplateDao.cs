using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IFieldTemplateDao
    {
        Task<List<FieldTemplate>> GetAllAsync(IncludeProperties includes = null);
        Task<FieldTemplate> CreateAsync(FieldTemplate model, IncludeProperties includes = null);
        Task<FieldTemplate> UpdateAsync(FieldTemplate model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}