using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IEventMappingDao
    {
        Task<EventMapping> GetAsync(int id, IncludeProperties includes = null);
        Task<List<EventMapping>> GetAllAsync(IncludeProperties includes = null, Expression<Func<EventMapping, bool>> filter = null);
        Task<EventMapping> CreateAsync(EventMapping model, IncludeProperties includes = null);
        Task<List<EventMapping>> CreateRangeAsync(List<EventMapping> models, IncludeProperties includes = null);
        Task<EventMapping> UpdateAsync(EventMapping model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}