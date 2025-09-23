using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface INotificationTypeDao
    {
        Task<List<NotificationType>> GetAllAsync(IncludeProperties includes = null);
    }
}