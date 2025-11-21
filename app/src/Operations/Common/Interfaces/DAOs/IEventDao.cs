using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IEventDao
    {
        Task<Event> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Event>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Event, bool>> filter = null);
        Task<Event> CreateAsync(Event model, IncludeProperties includes = null);
        Task<List<Event>> CreateRangeAsync(List<Event> models, IncludeProperties includes = null);
        Task<Event> UpdateAsync(Event model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}