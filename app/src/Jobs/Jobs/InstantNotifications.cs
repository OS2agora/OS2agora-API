using BallerupKommune.Models.Enums;
using BallerupKommune.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class InstantNotifications : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<InstantNotifications> _logger;

        public InstantNotifications(ISender mediator, ILogger<InstantNotifications> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var command = new CreateNotificationsAsNotificationQueueCommand
                {
                    Frequency = NotificationFrequency.INSTANT
                };
                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in scheduled job: {nameof(InstantNotifications)}. Error: {e.Message}");
            }
        }
    }
}