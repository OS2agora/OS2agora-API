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
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class NotificationTypeDao : BaseDao<NotificationTypeEntity, NotificationType>, INotificationTypeDao
    {
        public NotificationTypeDao(IApplicationDbContext db,
            ILogger<BaseDao<NotificationTypeEntity, NotificationType>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<NotificationType>> GetAllAsync(IncludeProperties includes = null)
        {
            var notificationTypeEntities = await base.GetAllAsync(includes);
            return notificationTypeEntities.Select(notificationTypeEntity => MapAndPrune(notificationTypeEntity, includes)).ToList();
        }
    }
}