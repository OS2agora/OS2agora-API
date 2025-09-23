using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface INotificationContentBuilder
    {
        Task<string> BuildStatusNotificationContent(List<Notification> notifications, User user);
        Task<List<string>> BuildNotificationContent(Notification notification);
    }
}
