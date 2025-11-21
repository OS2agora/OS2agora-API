using Agora.Operations.Common.Cleanup.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet;
using System;
using System.Threading.Tasks;
using DataCleanupJob = Jobs.Common.Constants.Jobs.DataCleanup;


namespace Jobs.Jobs
{
    public class DataCleanup : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<DataCleanup> _logger;
        private readonly IDistributedLockFactory _lockFactory;

        public DataCleanup(ISender mediator, ILogger<DataCleanup> logger, IDistributedLockFactory lockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _lockFactory = lockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            var lockTimeout = TimeSpan.FromSeconds(DataCleanupJob.LockTimeout);
            var waitTimeout = TimeSpan.FromSeconds(DataCleanupJob.WaitTimeout);
            var retryTime = TimeSpan.FromSeconds(DataCleanupJob.RetryTime);

            await using var redLock = await _lockFactory.CreateLockAsync(DataCleanupJob.JobIdentity,
                lockTimeout, waitTimeout, retryTime, context.CancellationToken);

            if (redLock.IsAcquired)
            {
                try
                {
                    var command = new GeneralCleanupCommand();
                    await _mediator.Send(command);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in scheduled job: {Job}. Error: {ErrorMessage}", nameof(DataCleanup), e.Message);
                }
            }

        }
    }
}
