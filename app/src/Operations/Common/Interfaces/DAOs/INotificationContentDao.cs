using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationContentDao
    {
        Task<NotificationContent> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationContent>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationContent, bool>> filter = null);
        Task<NotificationContent> CreateAsync(NotificationContent model, IncludeProperties includes = null);
        Task<List<NotificationContent>> CreateRangeAsync(List<NotificationContent> models, IncludeProperties includes = null);
        Task<NotificationContent> UpdateAsync(NotificationContent model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}