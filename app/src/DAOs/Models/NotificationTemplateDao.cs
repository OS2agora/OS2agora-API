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
    public class NotificationTemplateDao : BaseDao<NotificationTemplateEntity, NotificationTemplate>, INotificationTemplateDao
    {
        public NotificationTemplateDao(IApplicationDbContext db, ILogger<BaseDao<NotificationTemplateEntity, NotificationTemplate>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<NotificationTemplate> GetAsync(int id, IncludeProperties includes = null)
        {
            var entity = await base.GetAsync(id, includes);
            return MapAndPrune(entity, includes);
        }

        public async Task<List<NotificationTemplate>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationTemplate, bool>> filter = null)
        {
            var entities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<NotificationTemplate, NotificationTemplateEntity>());
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public new async Task<NotificationTemplate> CreateAsync(NotificationTemplate model, IncludeProperties includes = null)
        {
            var entity = await base.CreateAsync(model, includes);
            return MapAndPrune(entity, includes);
        }

        public new async Task<List<NotificationTemplate>> CreateRangeAsync(List<NotificationTemplate> models, IncludeProperties includes = null)
        {
            var entities = await base.CreateRangeAsync(models, includes);
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public async Task<NotificationTemplate> UpdateAsync(NotificationTemplate model, IncludeProperties includes = null)
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