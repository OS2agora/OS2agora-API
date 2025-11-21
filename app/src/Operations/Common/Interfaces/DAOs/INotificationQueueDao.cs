using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationQueueDao
    {
        Task<NotificationQueue> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationQueue>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationQueue, bool>> filter = null, int? limit = null, bool asNoTracking = false);
        Task<NotificationQueue> CreateAsync(NotificationQueue model, IncludeProperties includes = null);
        Task<List<NotificationQueue>> CreateRangeAsync(List<NotificationQueue> models, IncludeProperties includes = null);
        Task<NotificationQueue> UpdateAsync(NotificationQueue model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}