using System;
using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Mappings;

namespace BallerupKommune.DAOs.Models
{
    public class NotificationQueueDao : BaseDao<NotificationQueueEntity, NotificationQueue>, INotificationQueueDao
    {
        public NotificationQueueDao(IApplicationDbContext db,
            ILogger<BaseDao<NotificationQueueEntity, NotificationQueue>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public async Task<List<NotificationQueue>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<NotificationQueue, bool>> filter = null)
        {
            List<NotificationQueue> notificationQueueEntities = await base.GetAllAsync(includes,
                filter?.MapToEntityExpression<NotificationQueue, NotificationQueueEntity>());
            return notificationQueueEntities.Select(notificationQueueEntity => MapAndPrune(notificationQueueEntity, includes)).ToList();
        }

        public new async Task<NotificationQueue> CreateAsync(NotificationQueue model, IncludeProperties includes = null)
        {
            var notificationQueueEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(notificationQueueEntity, includes);
        }

        public async Task<NotificationQueue> UpdateAsync(NotificationQueue model, IncludeProperties includes = null)
        {
            var notificationQueueEntity = await UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(notificationQueueEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}