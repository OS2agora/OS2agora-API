using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationTemplateDao
    {
        Task<NotificationTemplate> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationTemplate>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationTemplate, bool>> filter = null);
        Task<NotificationTemplate> CreateAsync(NotificationTemplate model, IncludeProperties includes = null);
        Task<List<NotificationTemplate>> CreateRangeAsync(List<NotificationTemplate> models, IncludeProperties includes = null);
        Task<NotificationTemplate> UpdateAsync(NotificationTemplate model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}