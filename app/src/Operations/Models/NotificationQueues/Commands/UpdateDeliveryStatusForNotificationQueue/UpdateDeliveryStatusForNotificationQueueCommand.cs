using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Messages;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Agora.Operations.Models.NotificationQueues.Commands.UpdateDeliveryStatusForNotificationQueue
{
    public class UpdateDeliveryStatusForNotificationQueueCommand : IRequest<Unit>
    {
        public class UpdateDeliveryStatusForNotificationQueueCommandHandler : IRequestHandler<UpdateDeliveryStatusForNotificationQueueCommand, Unit>
        {
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IDigitalPostService _digitalPostService;
            private readonly ILogger<UpdateDeliveryStatusForNotificationQueueCommandHandler> _logger;
            private const int BatchSize = 1000;

            public UpdateDeliveryStatusForNotificationQueueCommandHandler(INotificationQueueDao notificationQueueDao, IDigitalPostService digitalPostService,
                ILogger<UpdateDeliveryStatusForNotificationQueueCommandHandler> logger)
            {
                _notificationQueueDao = notificationQueueDao;
                _digitalPostService = digitalPostService;
                _logger = logger;
            }

            public async Task<Unit> Handle(UpdateDeliveryStatusForNotificationQueueCommand request, CancellationToken cancellationToken)
            {
                var allDeliveryStatusUpdates = await _digitalPostService.GetMessageDeliveryStatus();

                if (!allDeliveryStatusUpdates.Any())
                {
                    return Unit.Value;
                }

                var deliveryStatusWithEmptyMessageId = allDeliveryStatusUpdates
                    .Where(deliveryStatus => string.IsNullOrEmpty(deliveryStatus.MessageId)).ToList();

                var messageIdsWithUpdates = allDeliveryStatusUpdates
                    .Where(deliveryStatus => !string.IsNullOrEmpty(deliveryStatus.MessageId))
                    .Select(deliveryStatus => deliveryStatus.MessageId)
                    .Distinct()
                    .ToList();

                // MySQL has a max limit of 2100 parameters per request - so we update in batches, to ensure we do not exceed the limit
                foreach (var batch in messageIdsWithUpdates.Batch(BatchSize))
                {
                    var notificationQueuesToUpdate = await _notificationQueueDao.GetAllAsync(null, queue => batch.Contains(queue.MessageId));
                    await UpdateDeliveryStatus(notificationQueuesToUpdate, allDeliveryStatusUpdates);
                }

                if (deliveryStatusWithEmptyMessageId.Any())
                {
                    HandleEmptyMessageIds(deliveryStatusWithEmptyMessageId);
                }

                return Unit.Value;
            }

            private async Task UpdateDeliveryStatus(List<NotificationQueue> notificationQueues, List<DeliveryStatus> allDeliveryStatusUpdates)
            {
                foreach (var notificationQueue in notificationQueues)
                {
                    var deliveryStatusUpdates = allDeliveryStatusUpdates
                        .Where(deliveryStatus => deliveryStatus.MessageId == notificationQueue.MessageId).ToList();
                    var latestDeliveryStatusUpdate = deliveryStatusUpdates.OrderByDescending(deliveryStatus => deliveryStatus.TransactionTime).First();

                    if (latestDeliveryStatusUpdate.Status != NotificationDeliveryStatus.FAILED && deliveryStatusUpdates.Any(status => status.Status == NotificationDeliveryStatus.FAILED || status.Errors.Any()))
                    {
                        _logger.LogWarning("Received status updates containing errors for NotificationQueue with Id '{Id}' and messageId '{MessageId}', but the newest delivery status is '{DeliveryStatus}'. Delivery Status will be updated using the latest update.", notificationQueue.Id, notificationQueue.MessageId, latestDeliveryStatusUpdate.Status);
                    }

                    var updatedNotificationQueue = notificationQueue;
                    updatedNotificationQueue.DeliveryStatus = latestDeliveryStatusUpdate.Status;
                    updatedNotificationQueue.SentAs = latestDeliveryStatusUpdate.SentAs;
                    updatedNotificationQueue.PropertiesUpdated = new List<string>
                    {
                        nameof(NotificationQueue.DeliveryStatus),
                        nameof(NotificationQueue.SentAs)
                    };

                    if (updatedNotificationQueue.DeliveryStatus == NotificationDeliveryStatus.FAILED)
                    {
                        var currentErrors = updatedNotificationQueue.ErrorTexts.ToList();
                        currentErrors.AddRange(latestDeliveryStatusUpdate.Errors);
                        updatedNotificationQueue.ErrorTexts = currentErrors.ToArray();
                        updatedNotificationQueue.PropertiesUpdated.Add(nameof(NotificationQueue.ErrorTexts));
                    }

                    if (updatedNotificationQueue.DeliveryStatus == NotificationDeliveryStatus.SUCCESSFUL)
                    {
                        updatedNotificationQueue.SuccessfulDeliveryDate = DateTime.Now;
                        updatedNotificationQueue.PropertiesUpdated.Add(nameof(NotificationQueue.SuccessfulDeliveryDate));
                    }

                    await _notificationQueueDao.UpdateAsync(updatedNotificationQueue);
                }
            }

            private void HandleEmptyMessageIds(List<DeliveryStatus> deliveryStatusWithEmptyMessageId)
            {
                foreach (var deliveryStatus in deliveryStatusWithEmptyMessageId)
                {
                    if (deliveryStatus.Errors.Any())
                    {
                        _logger.LogWarning("Received deliveryStatus with empty MessageId, so some NotificationQueue object may not be updated. The message was sent as {SentAs} with status {Status}, containing the following errors: {Errors}", deliveryStatus.SentAs, deliveryStatus.Status, string.Join("; ", deliveryStatus.Errors));
                    }
                    else
                    {
                        _logger.LogWarning("Received deliveryStatus with empty MessageId, so some NotificationQueue object may not be updated. The message was sent as {SentAs} with status {Status}", deliveryStatus.SentAs, deliveryStatus.Status);
                    }
                }
            }
        }
    }
}