using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationDao
    {
        Task<Notification> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Notification>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Notification, bool>> filter = null, bool asNoTracking = false);
        Task<Notification> CreateAsync(Notification model, IncludeProperties includes = null);
        Task<List<Notification>> CreateRangeAsync(List<Notification> models, IncludeProperties includes = null);
        Task<Notification> UpdateAsync(Notification model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}