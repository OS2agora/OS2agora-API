using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IHearingDao
    {
        Task<Hearing> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Hearing>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Hearing, bool>> filter = null);
        Task<Hearing> CreateAsync(Hearing model, IncludeProperties includes = null);
        Task<Hearing> UpdateAsync(Hearing model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}