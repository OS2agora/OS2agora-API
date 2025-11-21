using Agora.Operations.Models.Hearings.Command.UpdateHearingStatus;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;
using HearingStatusCheckerJob = Jobs.Common.Constants.Jobs.HearingStatusChecker;

namespace Jobs.Jobs
{
    public class HearingStatusChecker : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<HearingStatusChecker> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public HearingStatusChecker(ISender mediator, ILogger<HearingStatusChecker> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(HearingStatusCheckerJob.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(HearingStatusCheckerJob.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(HearingStatusCheckerJob.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(HearingStatusCheckerJob.JobIdentity,
                lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new UpdateHearingStatusCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(HearingStatusChecker), e.Message);
                }
            }
        }
    }
}