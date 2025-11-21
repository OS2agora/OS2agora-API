using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationContentTypeDao
    {
        Task<NotificationContentType> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationContentType>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationContentType, bool>> filter = null);
        Task<NotificationContentType> CreateAsync(NotificationContentType model, IncludeProperties includes = null);
        Task<List<NotificationContentType>> CreateRangeAsync(List<NotificationContentType> models, IncludeProperties includes = null);
        Task<NotificationContentType> UpdateAsync(NotificationContentType model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}