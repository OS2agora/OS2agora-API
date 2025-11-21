using Agora.Operations.Models.NotificationQueues.Commands.UpdateDeliveryStatusForNotificationQueue;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;
using UpdateNotificationQueueDeliveryStatusJob = Jobs.Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus;

namespace Jobs.Jobs
{
    public class UpdateNotificationQueueDeliveryStatus : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<UpdateNotificationQueueDeliveryStatus> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public UpdateNotificationQueueDeliveryStatus(ISender mediator, ILogger<UpdateNotificationQueueDeliveryStatus> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(UpdateNotificationQueueDeliveryStatusJob.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(UpdateNotificationQueueDeliveryStatusJob.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(UpdateNotificationQueueDeliveryStatusJob.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(UpdateNotificationQueueDeliveryStatusJob.JobIdentity,
                lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new UpdateDeliveryStatusForNotificationQueueCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {Message}", nameof(UpdateNotificationQueueDeliveryStatus), e.Message);
                }
            }
        }
    }
}