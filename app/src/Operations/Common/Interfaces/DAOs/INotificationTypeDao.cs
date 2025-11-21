using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationTypeDao
    {
        Task<NotificationType> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationType>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationType, bool>> filter = null);
        Task<NotificationType> CreateAsync(NotificationType model, IncludeProperties includes = null);
        Task<List<NotificationType>> CreateRangeAsync(List<NotificationType> models, IncludeProperties includes = null);
        Task<NotificationType> UpdateAsync(NotificationType model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}