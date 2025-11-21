using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.Interfaces.Notifications;
using Agora.Operations.Services.Notifications.Forms;
using Markdig;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;
using UserCapacity = Agora.Models.Enums.UserCapacity;

namespace Agora.Operations.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationBuilder _notificationBuilder;
        private readonly INotificationQueueDao _notificationQueueDao;
        private readonly IPdfService _pdfService;

        public NotificationService(INotificationBuilder notificationBuilder, INotificationQueueDao notificationQueueDao, IPdfService pdfService)
        {
            _notificationBuilder = notificationBuilder;
            _notificationQueueDao = notificationQueueDao;
            _pdfService = pdfService;
        }

        public async Task CreateAddedAsReviewerNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetAddedAsReviewerForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateInvitedToHearingNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetInvitedToHearingForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateHearingAnswerReceiptNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetHearingAnswerReceiptForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateHearingConclusionPublishedNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetHearingConclusionPublishedForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateHearingChangedNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetHearingChangedForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateHearingResponseDeclinedNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetHearingResponseDeclinedForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateDailyStatusNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetDailyStatusForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task CreateNewsLetterNotificationQueue(Notification notification)
        {
            var form = _notificationBuilder.GetNewsLetterForm();
            await BuildAndCreateNotificationQueue(notification, form);
        }

        public async Task<NotificationContentResult> GenerateEmailNotificationContentOfType(Hearing hearing,
            NotificationType notificationType)
        {
            var form = GetNotificationFormForType(notificationType);
            return await form.GetContentFromHearing(hearing);
        }

        public async Task<FileDownload> GenerateDownloadNotificationContentOfType(Hearing hearing, NotificationType notificationType)
        {
            var form = GetNotificationFormForType(notificationType);
            return await BuildAndCreateFileDownload(form, hearing);
        }

        private async Task<FileDownload> BuildAndCreateFileDownload(INotificationForm form, Hearing hearing)
        {
            var formContent = await form.GetContentFromHearing(hearing);
            var content = _pdfService.CreateTextPdf(formContent.Content, formContent.Subject, formContent.Subject);
            return new FileDownload
            {
                ContentType = "application/pdf",
                FileName = $"{formContent.Subject}.pdf",
                Content = content
            };
        }

        private async Task BuildAndCreateNotificationQueue(Notification notification, INotificationForm form)
        {
            var formContent = await form.GetContentFromNotification(notification);
            var messageChannel = GetNotificationMessageChannel(notification);
            var recipientAddress = GetRecipientAddress(notification, messageChannel);

            var content = CreateContent(notification, formContent.Content, formContent.Subject, messageChannel);

            await _notificationQueueDao.CreateAsync(new NotificationQueue
            {
                Content = content,
                Subject = formContent.Subject,
                RecipientAddress = recipientAddress,
                MessageChannel = messageChannel,
                NotificationId = notification.Id,
                DeliveryStatus = NotificationDeliveryStatus.AWAITING,
                IsSent = false
            });
        }

        private INotificationForm GetNotificationFormForType(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.ADDED_AS_REVIEWER:
                    return _notificationBuilder.GetAddedAsReviewerForm();
                case NotificationType.INVITED_TO_HEARING:
                    return _notificationBuilder.GetInvitedToHearingForm();
                case NotificationType.HEARING_ANSWER_RECEIPT:
                    return _notificationBuilder.GetHearingAnswerReceiptForm();
                case NotificationType.HEARING_CONCLUSION_PUBLISHED:
                    return _notificationBuilder.GetHearingConclusionPublishedForm();
                case NotificationType.HEARING_CHANGED:
                    return _notificationBuilder.GetHearingChangedForm();
                case NotificationType.HEARING_RESPONSE_DECLINED:
                    return _notificationBuilder.GetHearingResponseDeclinedForm();
                case NotificationType.DAILY_STATUS:
                    return _notificationBuilder.GetDailyStatusForm();
                case NotificationType.NEWSLETTER:
                    return _notificationBuilder.GetNewsLetterForm();
                case NotificationType.NONE:
                default:
                    throw new InvalidOperationException(
                        $"NotificationType of type '{notificationType}' is not a valid type");
            }
        }

        private static NotificationMessageChannel GetNotificationMessageChannel(Notification notification)
        {
            var company = notification.Company;
            var user = notification.User;
            if (user == null && company == null)
            {
                throw new Common.Exceptions.InvalidOperationException(
                    $"Cannot create NotificationQueue for Notification with Id '{notification.Id}'. No User or Company is defined.");
            }

            // If user is null, we want to send notification to company using eboks
            if (user == null)
            {
                return NotificationMessageChannel.EBOKS;
            }

            var userCapacity = user.UserCapacity.Capacity;
            switch (userCapacity)
            {
                case UserCapacity.CITIZEN:
                case UserCapacity.COMPANY:
                    return NotificationMessageChannel.EBOKS;
                case UserCapacity.EMPLOYEE:
                    return NotificationMessageChannel.EMAIL;
                case UserCapacity.NONE:
                default:
                    throw new ArgumentOutOfRangeException(nameof(userCapacity));
            }
        }

        private static string GetRecipientAddress(Notification notification, NotificationMessageChannel notificationMessageChannel)
        {
            var user = notification.User;
            var company = notification.Company;

            // If messageChannel is Email, we know we must send to a user with an email(employee)
            // If messageChannel is Eboks, we know the receiver is a company or a citizen.
            //      - If user is null, we know it must be a company, based on the checks done in GetNotificationMessageChannel
            switch (notificationMessageChannel)
            {
                case NotificationMessageChannel.EMAIL:
                    return user.Email;
                case NotificationMessageChannel.EBOKS:
                    return user == null ? company.Cvr : user.Cpr;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationMessageChannel));
            }
        }

        private string CreateContent(Notification notification, List<string> content, string subject, NotificationMessageChannel messageChannel)
        {
            if (notification.Company != null)
            {
                return CreateEboksContent(content, subject);
            }

            if (notification.User != null)
            {
                return CreateUserContent(notification, content, subject, messageChannel);
            }

            throw new Exception("Failed to send notifications. The notifications did not have a valid recipient");
        }

        private string CreateUserContent(Notification notification, List<string> content, string subject, NotificationMessageChannel notificationMessageChannel)
        {
            var user = notification.User;

            switch (notificationMessageChannel)
            {
                case NotificationMessageChannel.EMAIL:
                    return CreateEmailContent(content);
                case NotificationMessageChannel.EBOKS:
                    return CreateEboksContent(content, subject);
                default:
                    throw new Exception($"Cannot send notification to User with Id {user.Id}. Invalid messageChannel.");
            }
        }

        private string CreateEmailContent(List<string> content)
        {
            var contentStringBuilder = new StringBuilder();

            foreach (var singleContent in content)
            {
                contentStringBuilder.AppendLine(Markdown.ToHtml(singleContent));
            }

            return contentStringBuilder.ToString();
        }

        private string CreateEboksContent(List<string> content, string subject)
        {
            var pdfContent = _pdfService.CreateTextPdf(content, subject, subject);
            var pdfContentAsBase64String = Convert.ToBase64String(pdfContent);

            return pdfContentAsBase64String;
        }
    }
}