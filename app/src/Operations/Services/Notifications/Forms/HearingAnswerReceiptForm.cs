using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class HearingAnswerReceiptForm : BaseNotificationForm
    {
        private readonly IUserDao _userDao;

        public HearingAnswerReceiptForm(IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, IUserDao userDao, 
            IFieldSystemResolver fieldSystemResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
            : base(globalContentDao, notificationContentSpecificationDao, notificationTypeDao, fieldSystemResolver, textResolver, options)
        {
            _userDao = userDao;
        }

        public override Task<NotificationContentResult> GetContentFromHearing(Hearing hearing)
        {
            throw new NotImplementedException();
        }

        public override async Task<NotificationContentResult> GetContentFromNotification(Notification notification)
        {
            var contents = GetNotificationContents(notification.NotificationType);

            var subject = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.SUBJECT);
            var header = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.HEADER);
            var body = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.BODY);
            var footer = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.FOOTER);

            var companyResponder = "";
            if (notification.Company != null)
            {
                if (notification.UserId == null)
                {
                    throw new Exception($"Notification for Company with id {notification.Company.Id} does not contain userId. Cannot sent Hearing Answer Receipt.");
                }

                var commentResponder = await _userDao.GetAsync((int)notification.UserId);
                companyResponder = $" blev afgivet på vegne af jer af {commentResponder.Name}, og ";
            }

            var notificationContent = BuildStandardForm(header, body, footer);
            var contentWithReplacedCustomVariables = ReplaceCustomVariablesAsync(notificationContent, companyResponder);
            var contentWithReplacedCommonVariables = await ReplaceCommonVariablesAsync(contentWithReplacedCustomVariables, notification.Hearing);
            var subjectWithReplacedVariables = await ReplaceCommonVariablesAsync(subject, notification.Hearing);

            return new NotificationContentResult { Content = ReplaceNewLineVariable(contentWithReplacedCommonVariables), Subject = subjectWithReplacedVariables };
        }

        private static string ReplaceCustomVariablesAsync(string template, string companyResponder)
        {
            var result = template;

            result = result.Replace("{{CompanyResponder}}", companyResponder);

            return result;
        }
    }
}