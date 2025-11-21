using Agora.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class CreateNotificationQueue : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<CreateNotificationQueue> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public CreateNotificationQueue(ISender mediator, ILogger<CreateNotificationQueue> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateNotificationQueue.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateNotificationQueue.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateNotificationQueue.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(Common.Constants.Jobs.CreateNotificationQueue.JobIdentity,
                lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new CreateNotificationsAsNotificationQueueCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(CreateNotificationQueue), e.Message);
                }
            }
        }
    }
}