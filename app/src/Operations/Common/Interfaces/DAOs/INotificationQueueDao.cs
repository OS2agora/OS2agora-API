using System;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface INotificationQueueDao
    {
        Task<List<NotificationQueue>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<NotificationQueue, bool>> filter = null);
        Task<NotificationQueue> CreateAsync(NotificationQueue model, IncludeProperties includes = null);
        Task<NotificationQueue> UpdateAsync(NotificationQueue model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}