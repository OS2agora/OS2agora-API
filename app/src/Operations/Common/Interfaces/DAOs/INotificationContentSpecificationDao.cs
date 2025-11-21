using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface INotificationContentSpecificationDao
    {
        Task<NotificationContentSpecification> GetAsync(int id, IncludeProperties includes = null);
        Task<List<NotificationContentSpecification>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationContentSpecification, bool>> filter = null);
        Task<NotificationContentSpecification> CreateAsync(NotificationContentSpecification model, IncludeProperties includes = null);
        Task<List<NotificationContentSpecification>> CreateRangeAsync(List<NotificationContentSpecification> models, IncludeProperties includes = null);
        Task<NotificationContentSpecification> UpdateAsync(NotificationContentSpecification model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}