using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common
{
    public interface INotificationContentBuilder
    {
        Task<string> BuildStatusNotificationContent(List<Notification> notifications, User user);
        Task<List<string>> BuildNotificationContent(Notification notification);
    }
}
