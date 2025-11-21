using Agora.Operations.Models.NotificationQueues.Commands.SendNotificationQueue;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;
using NotificationQueueSenderJob = Jobs.Common.Constants.Jobs.NotificationQueueSender;

namespace Jobs.Jobs
{
    public class NotificationQueueSender : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<NotificationQueueSender> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public NotificationQueueSender(ISender mediator, ILogger<NotificationQueueSender> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(NotificationQueueSenderJob.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(NotificationQueueSenderJob.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(NotificationQueueSenderJob.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(NotificationQueueSenderJob.JobIdentity,
                             lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new SendNotificationQueuesCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(NotificationQueueSender), e.Message);
                }
            }
        }
    }
}