using Agora.Models.Enums;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Messages;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BodyType = Microsoft.Graph.Models.BodyType;

namespace Agora.DAOs.Messages.Email
{
    public class MsGraphEmailService : IEmailService
    {

        private readonly IOptions<AzureOptions> _azureOptions;
        private readonly IOptions<EmailOptions> _emailOptions;
        private readonly ILogger<MsGraphEmailService> _logger;

        public MsGraphEmailService(IOptions<AzureOptions> azureOptions, ILogger<MsGraphEmailService> logger, IOptions<EmailOptions> emailOptions)
        {
            _logger = logger;
            _emailOptions = emailOptions;
            _azureOptions = azureOptions;
        }

        public Task<NotificationSentReceipt> SendMessage(string subject, string content, string recipient)
        {
            var message = CreateMessage(subject, content, recipient);
            var client = CreateGraphServiceClient();

            try
            {
                var msGraphMailAddress = _emailOptions.Value.MsGraphMailAddress;
                _logger.LogInformation("Attempting to send notification via Ms Graph email from: {sender} to: {receiver}", msGraphMailAddress, recipient);
                var timeoutInSeconds = 30;
                var didFinish = client.Users[msGraphMailAddress].SendMail.PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody { Message = message })
                    .Wait(TimeSpan.FromSeconds(timeoutInSeconds));

                if (didFinish)
                {
                    return Task.FromResult(new NotificationSentReceipt
                    {
                        IsSent = true,
                        SentAs = NotificationSentAs.EMAIL,
                        DeliveryStatus = NotificationDeliveryStatus.SUCCESSFUL
                    });
                }

                throw new TimeoutException($"Failed to send message within the given time: {timeoutInSeconds} seconds");
            }
            catch (Exception e)
            {
                _logger.LogError("An error occured while sending email with subject: {subject} to recipient: {recipient}. Error message: {msg}", subject, recipient, e.Message);
                return Task.FromResult(new NotificationSentReceipt
                {
                    IsSent = false,
                    SentAs = NotificationSentAs.EMAIL,
                    Errors = new List<string> { e.Message }
                });
            }
        }

        private Message CreateMessage(string subject, string content, string address)
        {
            return new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = content
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = address
                        }
                    }
                }
            };
        }

        private GraphServiceClient CreateGraphServiceClient()
        {
            var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = _azureOptions.Value.ManagedIdentityClientId
            });

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var client = new GraphServiceClient(azureCredentials, scopes);
            return client;
        }
    }
}