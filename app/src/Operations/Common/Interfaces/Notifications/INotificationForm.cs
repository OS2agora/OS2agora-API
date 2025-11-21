using Agora.Models.Models;
using Agora.Operations.Services.Notifications.Forms;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Notifications
{
    public interface INotificationForm
    {
        Task<NotificationContentResult> GetContentFromNotification(Notification notification);
        Task<NotificationContentResult> GetContentFromHearing(Hearing hearing);
    }
}