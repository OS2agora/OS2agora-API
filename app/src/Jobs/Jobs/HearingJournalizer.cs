using BallerupKommune.Operations.Models.Hearings.Command.JournalizeHearings;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class HearingJournalizer : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<HearingJournalizer> _logger;

        public HearingJournalizer(ISender mediator, ILogger<HearingJournalizer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var command = new JournalizeHearingsCommand();
                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in scheduled job: {nameof(HearingJournalizer)}. Error: {e.Message}");
            }
        }
    }
}