using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface INotificationDao
    {
        Task<List<Notification>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Notification, bool>> filter = null);
        Task<Notification> CreateAsync(Notification model, IncludeProperties includes = null);
        Task<List<Notification>> CreateRangeAsync(List<Notification> models, IncludeProperties includes = null);
        Task<Notification> UpdateAsync(Notification model, IncludeProperties includes = null);
    }
}