using Agora.DAOs.Mappings;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class EventDao : BaseDao<EventEntity, Event>, IEventDao
    {
        public EventDao(IApplicationDbContext db, ILogger<BaseDao<EventEntity, Event>> logger, IMapper mapper,
            ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Event> GetAsync(int id, IncludeProperties includes = null)
        {
            var entity = await base.GetAsync(id, includes);
            return MapAndPrune(entity, includes);
        }

        public async Task<List<Event>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Event, bool>> filter = null)
        {
            var entities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<Event, EventEntity>());
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public new async Task<Event> CreateAsync(Event model, IncludeProperties includes = null)
        {
            var entity = await base.CreateAsync(model, includes);
            return MapAndPrune(entity, includes);
        }

        public new async Task<List<Event>> CreateRangeAsync(List<Event> models, IncludeProperties includes = null)
        {
            var entities = await base.CreateRangeAsync(models, includes);
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public async Task<Event> UpdateAsync(Event model, IncludeProperties includes = null)
        {
            var entity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(entity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}