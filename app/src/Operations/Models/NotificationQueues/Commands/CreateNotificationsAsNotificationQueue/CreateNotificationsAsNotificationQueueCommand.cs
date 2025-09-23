using BallerupKommune.Models.Enums;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using Markdig;
using Microsoft.Extensions.Logging;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;
using UserCapacity = BallerupKommune.Models.Enums.UserCapacity;
using NotificationType = BallerupKommune.Models.Models.NotificationType;

namespace BallerupKommune.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue
{
    public class CreateNotificationsAsNotificationQueueCommand : IRequest<Unit>
    {
        public NotificationFrequency Frequency { get; set; }

        public class
            CreateNotificationsAsNotificationQueueCommandHandler : IRequestHandler<
                CreateNotificationsAsNotificationQueueCommand, Unit>
        {
            private readonly INotificationDao _notificationDao;
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IPdfService _pdfService;
            private readonly ILogger<CreateNotificationsAsNotificationQueueCommandHandler> _logger;
            private readonly INotificationContentBuilder _notificationContentBuilder;

            public CreateNotificationsAsNotificationQueueCommandHandler(INotificationDao notificationDao,
                INotificationQueueDao notificationQueueDao, IPdfService pdfService, 
                ILogger<CreateNotificationsAsNotificationQueueCommandHandler> logger,
                INotificationContentBuilder notificationContentBuilder)
            {
                _notificationDao = notificationDao;
                _notificationQueueDao = notificationQueueDao;
                _pdfService = pdfService;
                _logger = logger;
                _notificationContentBuilder = notificationContentBuilder;
            }

            public async Task<Unit> Handle(CreateNotificationsAsNotificationQueueCommand request,
                CancellationToken cancellationToken)
            {
                var notificationIncludes = IncludeProperties.Create<Notification>(null,
                    new List<string>
                    {
                        nameof(Notification.Hearing),
                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.HearingType)}",
                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.HearingStatus)}",

                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.Contents)}",
                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",

                        nameof(Notification.Comment),
                        $"{nameof(Notification.Comment)}.{nameof(Comment.User)}",

                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.UserHearingRoles)}",
                        $"{nameof(Notification.Hearing)}.{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}",

                        nameof(Notification.User),
                        $"{nameof(Notification.User)}.{nameof(User.UserCapacity)}",
                        nameof(Notification.Company),

                        nameof(Notification.NotificationType),
                        $"{nameof(Notification.NotificationType)}.{nameof(NotificationType.NotificationTemplate)}"
                    });

                List<Notification> notificationsToHandle =
                    await _notificationDao.GetAllAsync(notificationIncludes, GetRequestFilter(request.Frequency));

                if (request.Frequency == NotificationFrequency.DAILY)
                {
                    await HandleDailyNotifications(notificationsToHandle);
                }

                if (request.Frequency == NotificationFrequency.INSTANT)
                {
                    foreach (var notification in notificationsToHandle)
                    {
                        try
                        {
                            await HandleInstantNotification(notification);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception.Message);
                        }
                    }
                }

                return Unit.Value;
            }

            private async Task HandleDailyNotifications(List<Notification> notifications)
            {
                var groupedByUser = notifications.GroupBy(notification => notification.User.Id);

                foreach (var userGroup in groupedByUser)
                {
                    try
                    {
                        var notificationsForUser = userGroup.ToList();
                        var user = notificationsForUser.First().User;

                        var content = await _notificationContentBuilder.BuildStatusNotificationContent(notificationsForUser, user);
                        var subject = "Høringsportalen: Det seneste døgns aktiviteter";

                        await SendNotifications(notificationsForUser, new List<string>{ content }, subject);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.Message);
                    }
                }
            }

            private async Task HandleInstantNotification(Notification notification)
            {
                var isInvitationNotification = notification.NotificationType.Type ==
                                               BallerupKommune.Models.Enums.NotificationType.INVITED_TO_HEARING;
                var isHearingPublished = notification.Hearing.HearingStatus.Status != HearingStatus.DRAFT &&
                                      notification.Hearing.HearingStatus.Status != HearingStatus.CREATED;

                if (isInvitationNotification && !isHearingPublished)
                {
                    return;
                }

                var subject = notification.NotificationType.NotificationTemplate.SubjectTemplate;
                var content = await _notificationContentBuilder.BuildNotificationContent(notification);
                await SendNotifications(new List<Notification> { notification }, content, subject);
            }

            private async Task SendNotifications(List<Notification> notifications, List<string> content, string subject)
            {
                if (notifications.First().Company != null)
                {
                    await SendCompanyNotifications(notifications, content, subject);
                }
                else if (notifications.First().User != null)
                {
                    await SendUserNotifications(notifications, content, subject);
                }

                throw new Exception($"Failed to send notifications. The notifications did not have a valid recipient");
            }

            private async Task SendUserNotifications(List<Notification> notifications, List<string> content, string subject)
            {
                var user = notifications.First().User;
                var messageChannel = GetNotificationChannel(user.UserCapacity.Capacity);

                switch (messageChannel)
                {
                    case NotificationMessageChannel.EMAIL:
                        if (string.IsNullOrEmpty(user.Email))
                        {
                            throw new Exception(
                                $"Cannot send notification as email to User with id {user.Id}. User does not have an email");
                        }
                        await SendEmailNotifications(user.Email, notifications, content, subject);
                        break;
                    case NotificationMessageChannel.EBOKS:
                        var recipientAddress = user.Cpr;
                        if (user.UserCapacity.Capacity == UserCapacity.COMPANY)
                        {
                            _logger.LogWarning($"User with id {user.Id} has UserCapacity {nameof(UserCapacity.COMPANY)}. Changing recipientAddress for notification to CVR.");
                            recipientAddress = user.Cvr;
                        }

                        if (string.IsNullOrEmpty(recipientAddress))
                        {
                            throw new Exception(
                                $"Cannot send notification via eboks to User with id {user.Id}. User does not have a valid recipientAddress");
                        }

                        await SendEboksNotifications(recipientAddress, notifications, content, subject);
                        break;
                    default:
                        throw new Exception(
                            $"Cannot send notification to User with Id {user.Id}. Invalid messageChannel.");
                }
            }

            private async Task SendCompanyNotifications(List<Notification> notifications, List<string> content, string subject)
            {
                var company = notifications.First().Company;
                await SendEboksNotifications(company.Cvr, notifications, content, subject);
            }

            private async Task SendEmailNotifications(string email, List<Notification> notifications, List<string> content, string subject)
            {
                var contentStringBuilder = new StringBuilder();

                foreach (var singleContent in content)
                {
                    contentStringBuilder.AppendLine(Markdown.ToHtml(singleContent));
                }

                var createdNotificationQueue = await CreateNotificationQueue(subject,
                    contentStringBuilder.ToString(), email, NotificationMessageChannel.EMAIL);
                await UpdateNotifications(notifications, createdNotificationQueue);
            }

            private async Task SendEboksNotifications(string recipientAddress, List<Notification> notifications, List<string> content, string subject)
            {
                var pdfContent = _pdfService.CreateTextPdf(content, subject, subject);
                var pdfContentAsBase64String = Convert.ToBase64String(pdfContent);

                var createdNotificationQueue = await CreateNotificationQueue(subject, pdfContentAsBase64String,
                    recipientAddress, NotificationMessageChannel.EBOKS);
                await UpdateNotifications(notifications, createdNotificationQueue);
            }

            private async Task UpdateNotifications(List<Notification> notifications,
                NotificationQueue createdNotificationQueue)
            {
                foreach (var notification in notifications)
                {
                    var updatedNotification = notification;
                    updatedNotification.IsSendToQueue = true;
                    updatedNotification.NotificationQueueId = createdNotificationQueue.Id;
                    updatedNotification.NotificationQueue = createdNotificationQueue;
                    updatedNotification.PropertiesUpdated = new List<string> {nameof(Notification.IsSendToQueue)};

                    await _notificationDao.UpdateAsync(updatedNotification);
                }
            }

            private async Task<NotificationQueue> CreateNotificationQueue(string subject, string content,
                string recipientAddress, NotificationMessageChannel messageChannel)
            {
                return await _notificationQueueDao.CreateAsync(
                    new NotificationQueue
                    {
                        Content = content,
                        MessageChannel = messageChannel,
                        RecipientAddress = recipientAddress,
                        Subject = subject,
                    });
            }

            public NotificationMessageChannel GetNotificationChannel(UserCapacity userCapacity)
            {
                switch (userCapacity)
                {
                    case UserCapacity.CITIZEN:
                    case UserCapacity.COMPANY:
                        return NotificationMessageChannel.EBOKS;
                    case UserCapacity.EMPLOYEE:
                        return NotificationMessageChannel.EMAIL;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static Expression<Func<Notification, bool>> GetRequestFilter(NotificationFrequency frequency)
            {
                return frequency switch
                {
                    NotificationFrequency.INSTANT => notification =>
                        !notification.IsSendToQueue &&
                        notification.NotificationType.Frequency == NotificationFrequency.INSTANT,
                    NotificationFrequency.DAILY => notification =>
                        !notification.IsSendToQueue &&
                        notification.NotificationType.Frequency == NotificationFrequency.DAILY,
                    _ => throw new ArgumentOutOfRangeException(nameof(frequency))
                };
            }
        }
    }
}