using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using NotificationContentType = Agora.Models.Enums.NotificationContentType;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class NewsLetterForm : BaseNotificationForm
    {
        public NewsLetterForm(IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, 
            IFieldSystemResolver fieldSystemResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
            : base(globalContentDao, notificationContentSpecificationDao, notificationTypeDao, fieldSystemResolver, textResolver, options)
        {
        }

        public override Task<NotificationContentResult> GetContentFromHearing(Hearing hearing)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<NotificationContentResult> GetContentFromNotification(Notification notification)
        {
            var contents = GetNotificationContents(notification.NotificationType);

            var subject = GetContentByType(contents, NotificationContentType.SUBJECT);
            var footer = GetContentByType(contents, NotificationContentType.FOOTER);

            var dailyStatusContent = await BuildDailyStatusContent(notification);
            var notificationContent = dailyStatusContent + footer;
            var contentWithReplacedVariables = await ReplaceCommonVariablesAsync(notificationContent, notification.Hearing);
            var subjectWithReplacedVariables = await ReplaceCommonVariablesAsync(subject, notification.Hearing);

            return new NotificationContentResult { Content = ReplaceNewLineVariable(contentWithReplacedVariables), Subject = subjectWithReplacedVariables };
        }

        // TODO: Implement this once we get NewsLetters
        private async Task<string> BuildDailyStatusContent(Notification notification)
        {
            return "{{NewsLetter}}";
        }
    }
}