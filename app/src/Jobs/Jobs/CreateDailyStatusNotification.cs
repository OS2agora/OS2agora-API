using Agora.Operations.Models.Notifications.Commands.CreateDailyStatusNotification;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class CreateDailyStatusNotification : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<CreateDailyStatusNotification> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public CreateDailyStatusNotification(ISender mediator, ILogger<CreateDailyStatusNotification> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateDailyStatusNotifications.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateDailyStatusNotifications.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(Common.Constants.Jobs.CreateDailyStatusNotifications.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(Common.Constants.Jobs.CreateDailyStatusNotifications.JobIdentity, lockTimeout,
                waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new CreateDailyStatusNotificationCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(CreateDailyStatusNotification), e.Message);
                }
            }
        }
    }
}