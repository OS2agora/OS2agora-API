using Agora.Models.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Messages;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Messages.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IFluentEmailFactory _emailFactory;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IFluentEmailFactory emailFactory, ILogger<SmtpEmailService> logger)
        {
            _emailFactory = emailFactory;
            _logger = logger;
        }

        public async Task<NotificationSentReceipt> SendMessage(string subject, string content, string recipient)
        {
            var email = await _emailFactory.Create().To(recipient).Subject(subject).Body(content, true).SendAsync();

            if (email.Successful)
            {
                return new NotificationSentReceipt
                {
                    IsSent = true,
                    SentAs = NotificationSentAs.EMAIL,
                    DeliveryStatus = NotificationDeliveryStatus.SUCCESSFUL
                };
            }

            if (email.ErrorMessages.Count > 0)
            {
                foreach (var errorMessage in email.ErrorMessages)
                {
                    _logger.LogError(
                        $"Email Service: Error caught when sending e-mail with ID: {email.MessageId}. Error: {errorMessage}");
                }
            }

            return new NotificationSentReceipt
            {
                IsSent = false,
                SentAs = NotificationSentAs.EMAIL,
                Errors = email.ErrorMessages.ToList()
            };
        }
    }
}