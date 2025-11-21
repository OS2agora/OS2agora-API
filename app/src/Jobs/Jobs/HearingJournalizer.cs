using Agora.Operations.Models.Hearings.Command.JournalizeHearings;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;
using HearingJournalizerJob = Jobs.Common.Constants.Jobs.HearingJournalizer;

namespace Jobs.Jobs
{
    public class HearingJournalizer : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<HearingJournalizer> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public HearingJournalizer(ISender mediator, ILogger<HearingJournalizer> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lockTimeout = TimeSpan.FromSeconds(HearingJournalizerJob.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(HearingJournalizerJob.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(HearingJournalizerJob.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(HearingJournalizerJob.JobIdentity,
                lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new JournalizeHearingsCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(HearingJournalizer), e.Message);
                }
            }
        }
    }
}