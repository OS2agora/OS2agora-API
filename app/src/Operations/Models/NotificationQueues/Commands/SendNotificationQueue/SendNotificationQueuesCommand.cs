using BallerupKommune.Models.Enums;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Models.NotificationQueues.Commands.SendNotificationQueue
{
    public class SendNotificationQueuesCommand : IRequest<Unit>
    {
        public class SendNotificationQueuesCommandHandler : IRequestHandler<SendNotificationQueuesCommand, Unit>
        {
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IEmailService _emailService;
            private readonly IEBoksService _eBoksService;

            public SendNotificationQueuesCommandHandler(INotificationQueueDao notificationQueueDao,
                IEmailService emailService, IEBoksService eBoksService)
            {
                _notificationQueueDao = notificationQueueDao;
                _emailService = emailService;
                _eBoksService = eBoksService;
            }

            public async Task<Unit> Handle(SendNotificationQueuesCommand request, CancellationToken cancellationToken)
            {
                List<NotificationQueue> notificationQueuesThatShouldBeSent =
                    await _notificationQueueDao.GetAllAsync(null, queue => !queue.IsSend && queue.RetryCount < 10);

                foreach (var notificationQueue in notificationQueuesThatShouldBeSent)
                {
                    var recipient = notificationQueue.RecipientAddress;
                    var subject = notificationQueue.Subject;
                    var content = notificationQueue.Content;

                    var successfullySent = false;
                    var errorMessage = new List<string>();

                    if (notificationQueue.MessageChannel == NotificationMessageChannel.EBOKS)
                    {
                        try
                        {
                            successfullySent = await _eBoksService.SendMessage(subject, content, recipient);
                        }
                        catch (Exception e)
                        {
                            errorMessage.Add(e.Message);
                        }
                    }

                    if (notificationQueue.MessageChannel == NotificationMessageChannel.EMAIL)
                    {
                        try
                        {
                            successfullySent = await _emailService.SendMessage(subject, content, recipient);
                        }
                        catch (EmailException e)
                        {
                            errorMessage.AddRange(e.Errors);
                        }
                        catch (Exception e)
                        {
                            errorMessage.Add(e.Message);
                        }
                    }

                    if (successfullySent)
                    {
                        var updatedNotificationQueue = notificationQueue;
                        updatedNotificationQueue.IsSend = true;
                        updatedNotificationQueue.SuccessfullSendDate = DateTime.Now;
                        updatedNotificationQueue.PropertiesUpdated = new List<string>
                        {
                            nameof(NotificationQueue.IsSend),
                            nameof(NotificationQueue.SuccessfullSendDate)
                        };
                        await _notificationQueueDao.UpdateAsync(updatedNotificationQueue);
                    }
                    else
                    {
                        var updatedNotificationQueue = notificationQueue;

                        var currentErrors = updatedNotificationQueue.ErrorTexts.ToList();
                        currentErrors.AddRange(errorMessage);

                        updatedNotificationQueue.ErrorTexts = currentErrors.ToArray();
                        updatedNotificationQueue.RetryCount += 1;
                        updatedNotificationQueue.PropertiesUpdated = new List<string>
                        {
                            nameof(NotificationQueue.ErrorTexts),
                            nameof(NotificationQueue.RetryCount)
                        };
                        await _notificationQueueDao.UpdateAsync(updatedNotificationQueue);
                    }
                }

                return Unit.Value;
            }
        }
    }
}