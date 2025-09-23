using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Mappings;

namespace BallerupKommune.DAOs.Models
{
    public class NotificationDao : BaseDao<NotificationEntity, Notification>, INotificationDao
    {
        public NotificationDao(IApplicationDbContext db, ILogger<BaseDao<NotificationEntity, Notification>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public async Task<List<Notification>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Notification, bool>> filter = null)
        {
            List<Notification> notificationEntities = await base.GetAllAsync(includes,
                filter?.MapToEntityExpression<Notification, NotificationEntity>());
            return notificationEntities.Select(notificationEntity => MapAndPrune(notificationEntity, includes)).ToList();
        }

        public new async Task<Notification> CreateAsync(Notification model, IncludeProperties includes = null)
        {
            var notificationEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(notificationEntity, includes);
        }

        public new async Task<List<Notification>> CreateRangeAsync(List<Notification> models,
            IncludeProperties includes = null)
        {
            var notificationEntities = await base.CreateRangeAsync(models, includes);
            return notificationEntities.Select(notificationEntity => MapAndPrune(notificationEntity, includes)).ToList();
        }

        public async Task<Notification> UpdateAsync(Notification model, IncludeProperties includes = null)
        {
            var notificationEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(notificationEntity, includes);
        }
    }
}