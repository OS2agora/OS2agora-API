using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions.OperationsOptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Messages;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.NotificationQueues.Commands.SendNotificationQueue
{
    public class SendNotificationQueuesCommand : IRequest<Unit>
    {
        public class SendNotificationQueuesCommandHandler : IRequestHandler<SendNotificationQueuesCommand, Unit>
        {
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IEmailService _emailService;
            private readonly IDigitalPostService _digitalPostService;
            private readonly int _maxRetryCount;
            private readonly int _notificationQueuesToHandle;

            public SendNotificationQueuesCommandHandler(INotificationQueueDao notificationQueueDao,
                IEmailService emailService, IDigitalPostService digitalPostService, IOptions<NotificationQueueOperationsOptions> options)
            {
                _notificationQueueDao = notificationQueueDao;
                _emailService = emailService;
                _digitalPostService = digitalPostService;
                _maxRetryCount = options.Value.SendNotificationQueues.MaxRetryCount;
                _notificationQueuesToHandle = options.Value.SendNotificationQueues.NotificationQueuesToHandle;
            }

            public async Task<Unit> Handle(SendNotificationQueuesCommand request, CancellationToken cancellationToken)
            {
                // MapToEntityExpression throws error if we read directly from _maxRetryCount
                var maxRetryCount = _maxRetryCount;
                List<NotificationQueue> notificationQueuesThatShouldBeSent =
                    await _notificationQueueDao.GetAllAsync(null, queue => queue.RetryCount < maxRetryCount && (!queue.IsSent || queue.DeliveryStatus == NotificationDeliveryStatus.FAILED), _notificationQueuesToHandle, asNoTracking: true);

                foreach (var notificationQueue in notificationQueuesThatShouldBeSent)
                {
                    var receipt = await SendNotificationQueue(notificationQueue);

                    if (receipt.IsSent)
                    {
                        var currentTime = DateTime.Now;

                        var updatedNotificationQueue = notificationQueue;
                        updatedNotificationQueue.IsSent = receipt.IsSent;
                        updatedNotificationQueue.SentAs = receipt.SentAs;
                        updatedNotificationQueue.MessageId = receipt.MessageId;
                        updatedNotificationQueue.DeliveryStatus = receipt.DeliveryStatus;
                        updatedNotificationQueue.SuccessfulSentDate = currentTime;
                        updatedNotificationQueue.PropertiesUpdated = new List<string>
                        {
                            nameof(NotificationQueue.IsSent),
                            nameof(NotificationQueue.SuccessfulSentDate),
                            nameof(NotificationQueue.SentAs),
                            nameof(NotificationQueue.MessageId),
                            nameof(NotificationQueue.DeliveryStatus)
                        };

                        if (updatedNotificationQueue.DeliveryStatus == NotificationDeliveryStatus.SUCCESSFUL)
                        {
                            updatedNotificationQueue.SuccessfulDeliveryDate = currentTime;
                            updatedNotificationQueue.PropertiesUpdated.Add(nameof(NotificationQueue.SuccessfulDeliveryDate));
                        }

                        await _notificationQueueDao.UpdateAsync(updatedNotificationQueue);
                    }
                    else
                    {
                        var updatedNotificationQueue = notificationQueue;

                        var currentErrors = updatedNotificationQueue.ErrorTexts.ToList();

                        currentErrors.AddRange(receipt.Errors);

                        updatedNotificationQueue.SentAs = receipt.SentAs;
                        updatedNotificationQueue.ErrorTexts = currentErrors.ToArray();
                        updatedNotificationQueue.RetryCount += 1;
                        updatedNotificationQueue.PropertiesUpdated = new List<string>
                        {
                            nameof(NotificationQueue.SentAs),
                            nameof(NotificationQueue.ErrorTexts),
                            nameof(NotificationQueue.RetryCount)
                        };

                        if (updatedNotificationQueue.RetryCount >= _maxRetryCount)
                        {
                            updatedNotificationQueue.DeliveryStatus = NotificationDeliveryStatus.FAILED;
                            updatedNotificationQueue.PropertiesUpdated.Add(nameof(NotificationQueue.DeliveryStatus));
                        }

                        await _notificationQueueDao.UpdateAsync(updatedNotificationQueue);
                    }
                }

                return Unit.Value;
            }

            private async Task<NotificationSentReceipt> SendNotificationQueue(NotificationQueue notificationQueue)
            {
                var recipient = notificationQueue.RecipientAddress;
                var subject = notificationQueue.Subject;
                var content = notificationQueue.Content;

                if (notificationQueue.MessageChannel == NotificationMessageChannel.EBOKS)
                {
                    return await _digitalPostService.SendMessage(notificationQueue.Id, subject, content, recipient);
                }

                if (notificationQueue.MessageChannel == NotificationMessageChannel.EMAIL)
                {
                    return await _emailService.SendMessage(subject, content, recipient);
                }

                return new NotificationSentReceipt
                {
                    IsSent = false,
                    SentAs = NotificationSentAs.UNKNOWN,
                    Errors = new List<string>
                    {
                        $"Invalid MessageChannel. The provided MessageChannel was '{notificationQueue.MessageChannel}'"
                    }
                };
            }
        }
    }
}