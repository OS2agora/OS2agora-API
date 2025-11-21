using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class HearingChangedForm : BaseNotificationForm
    {
        public HearingChangedForm(IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, 
            IFieldSystemResolver fieldSystemResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
            : base(globalContentDao, notificationContentSpecificationDao, notificationTypeDao, fieldSystemResolver, textResolver, options)
        {
        }

        public override Task<NotificationContentResult> GetContentFromHearing(Hearing hearing)
        {
            return GetStandardNotificationContent(hearing, NotificationType.HEARING_CHANGED);
        }

        public override Task<NotificationContentResult> GetContentFromNotification(Notification notification)
        {
            return GetStandardNotificationContentFromTemplate(notification);
        }
    }
}