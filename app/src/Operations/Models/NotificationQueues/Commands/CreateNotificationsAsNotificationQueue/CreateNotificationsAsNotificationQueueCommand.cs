using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue
{
    public class CreateNotificationsAsNotificationQueueCommand : IRequest<Unit>
    {
        public class CreateNotificationsAsNotificationQueueCommandHandler : IRequestHandler<CreateNotificationsAsNotificationQueueCommand, Unit>
        {
            private readonly INotificationDao _notificationDao;
            private readonly INotificationService _notificationService;
            private readonly ILogger<CreateNotificationsAsNotificationQueueCommand> _logger;

            public CreateNotificationsAsNotificationQueueCommandHandler(INotificationDao notificationDao, INotificationService notificationService, ILogger<CreateNotificationsAsNotificationQueueCommand> logger)
            {
                _notificationDao = notificationDao;
                _notificationService = notificationService;
                _logger = logger;
            }

            public async Task<Unit> Handle(CreateNotificationsAsNotificationQueueCommand request, CancellationToken cancellationToken)
            {
                var notificationsToHandle = await _notificationDao.GetAllAsync(NotificationIncludes, n => !n.IsSentToQueue, asNoTracking: false);

                if (!notificationsToHandle.Any())
                {
                    return Unit.Value;
                }

                foreach (var notification in notificationsToHandle)
                {
                    try
                    {
                        await HandleNotification(notification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to Create NotificationQueue for notification with Id {Id}", notification.Id);
                    }
                }

                return Unit.Value;
            }

            private async Task HandleNotification(Notification notification)
            {
                if (notification.NotificationQueue != null)
                {
                    notification.IsSentToQueue = true;
                    notification.PropertiesUpdated = new List<string> { nameof(Notification.IsSentToQueue) };
                    await _notificationDao.UpdateAsync(notification);
                    return;
                }

                switch (notification.NotificationType.Type)
                {
                    case Agora.Models.Enums.NotificationType.ADDED_AS_REVIEWER:
                        await _notificationService.CreateAddedAsReviewerNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.INVITED_TO_HEARING:
                        await _notificationService.CreateInvitedToHearingNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.HEARING_ANSWER_RECEIPT:
                        await _notificationService.CreateHearingAnswerReceiptNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.HEARING_CONCLUSION_PUBLISHED:
                        await _notificationService.CreateHearingConclusionPublishedNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.HEARING_CHANGED:
                        await _notificationService.CreateHearingChangedNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.HEARING_RESPONSE_DECLINED:
                        await _notificationService.CreateHearingResponseDeclinedNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.DAILY_STATUS:
                        await _notificationService.CreateDailyStatusNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.NEWSLETTER:
                        await _notificationService.CreateNewsLetterNotificationQueue(notification);
                        break;
                    case Agora.Models.Enums.NotificationType.NONE:
                    default:
                        _logger.LogWarning("Unknown notification type encountered: '{NotificationType}'. Can't create notification.", notification.NotificationType.Type);
                        break;
                }

                notification.IsSentToQueue = true;
                notification.PropertiesUpdated = new List<string> { nameof(Notification.IsSentToQueue) };
                await _notificationDao.UpdateAsync(notification);
            }

            private static IncludeProperties NotificationIncludes
            {
                get
                {
                    var notificationIncludes = IncludeProperties.Create<Notification>(null,
                        new List<string>
                        {
                            // Requirements for this handler
                            nameof(Notification.NotificationQueue),
                            nameof(Notification.NotificationType),
                            $"{nameof(Notification.NotificationType)}.{nameof(NotificationType.SubjectTemplate)}",
                            $"{nameof(Notification.NotificationType)}.{nameof(NotificationType.HeaderTemplate)}",
                            $"{nameof(Notification.NotificationType)}.{nameof(NotificationType.BodyTemplate)}",
                            $"{nameof(Notification.NotificationType)}.{nameof(NotificationType.FooterTemplate)}",
                            nameof(Notification.User),
                            $"{nameof(Notification.User)}.{nameof(User.UserCapacity)}",
                            nameof(Notification.Company),
                            nameof(Notification.Hearing),
                            $"{nameof(Notification.Hearing)}.{nameof(Hearing.HearingType)}",
                            $"{nameof(Notification.Hearing)}.{nameof(Hearing.Contents)}",
                            $"{nameof(Notification.Hearing)}.{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                        });
                    return notificationIncludes;
                }
            }
        }
    }
}