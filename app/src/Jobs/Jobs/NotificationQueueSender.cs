using BallerupKommune.Operations.Models.NotificationQueues.Commands.SendNotificationQueue;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Jobs.Jobs
{
    public class NotificationQueueSender : IJob
    {
        private readonly ISender _mediator;
        private readonly ILogger<NotificationQueueSender> _logger;

        public NotificationQueueSender(ISender mediator, ILogger<NotificationQueueSender> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var command = new SendNotificationQueuesCommand();
                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in scheduled job: {nameof(NotificationQueueSender)}. Error: {e.Message}");
            }
        }
    }
}