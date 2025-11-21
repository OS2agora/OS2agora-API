using Agora.Models.Models;
using Agora.Operations.Services.Notifications.Forms;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Common.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task CreateAddedAsReviewerNotificationQueue(Notification notification);
        Task CreateInvitedToHearingNotificationQueue(Notification notification);
        Task CreateHearingAnswerReceiptNotificationQueue(Notification notification);
        Task CreateHearingConclusionPublishedNotificationQueue(Notification notification);
        Task CreateHearingChangedNotificationQueue(Notification notification);
        Task CreateHearingResponseDeclinedNotificationQueue(Notification notification);
        Task CreateDailyStatusNotificationQueue(Notification notification);
        Task CreateNewsLetterNotificationQueue(Notification notification);

        Task<FileDownload> GenerateDownloadNotificationContentOfType(Hearing hearing, NotificationType notificationType);
        Task<NotificationContentResult> GenerateEmailNotificationContentOfType(Hearing hearing, NotificationType notificationType);
    }
}