using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IHearingDao
    {
        Task<Hearing> GetAsync(int id, IncludeProperties includes = null, bool asNoTracking = false);
        Task<List<Hearing>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Hearing, bool>> filter = null);
        Task<Hearing> CreateAsync(Hearing model, IncludeProperties includes = null);
        Task<Hearing> UpdateAsync(Hearing model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}