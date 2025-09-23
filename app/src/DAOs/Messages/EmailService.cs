using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Messages
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmailFactory _emailFactory;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmailFactory emailFactory, ILogger<EmailService> logger)
        {
            _emailFactory = emailFactory;
            _logger = logger;
        }


        public async Task<bool> SendMessage(string subject, string content, string recipient)
        {
            var email = await _emailFactory.Create().To(recipient).Subject(subject).Body(content, true).SendAsync();

            if (email.Successful)
            {
                return true;
            }

            if (email.ErrorMessages.Count > 0)
            {
                foreach (var errorMessage in email.ErrorMessages)
                {
                    _logger.LogError(
                        $"Email Service: Error caught when sending e-mail with ID: {email.MessageId}. Error: {errorMessage}");
                }

                throw new EmailException(email.ErrorMessages);
            }

            return false;
        }
    }
}