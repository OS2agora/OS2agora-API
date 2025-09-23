using BallerupKommune.Operations.Models.Hearings.Command.UpdateHearingStatus;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class HearingStatusChecker : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<HearingStatusChecker> _logger;

        public HearingStatusChecker(ISender mediator, ILogger<HearingStatusChecker> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var command = new UpdateHearingStatusCommand();
                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in scheduled job: {nameof(HearingStatusChecker)}. Error: {e.Message}");
            }
        }
    }
}