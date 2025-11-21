using Agora.DAOs.Messages.RemotePrint;
using Agora.DAOs.Messages.RemotePrint.Constants;
using Agora.DAOs.Messages.RemotePrint.DTOs;
using Agora.Models.Enums;
using Agora.Models.Models.Cpr;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.Cpr;
using Agora.Operations.Common.Interfaces.Cvr;
using Agora.Operations.Common.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Messages
{
    public class RemotePrintService : IDigitalPostService
    {
        private readonly RemotePrintClient _client;
        private readonly ICprInformationService _cprInformationService;
        private readonly ICvrInformationService _cvrInformationService;
        private readonly ILogger<RemotePrintService> _logger;

        public RemotePrintService(RemotePrintClient client, ICprInformationService cprInformationService, ICvrInformationService cvrInformationService, ILogger<RemotePrintService> logger)
        {
            _client = client;
            _cprInformationService = cprInformationService;
            _cvrInformationService = cvrInformationService;
            _logger = logger;
        }

        public async Task<List<DeliveryStatus>> GetMessageDeliveryStatus()
        {
            try
            {
                var responseList = await _client.GetMessageStatus();
                return responseList.Select(MapStatusResponseDtoToNotificationStatus).ToList();
            }
            catch (Exception ex)
            {
                return new List<DeliveryStatus>();
            }
            
        }

        public async Task<NotificationSentReceipt> SendMessage(int queueId, string subject, string content, string recipient, 
            string name = null, 
            string address = null,
            string postalCode = null, 
            string city = null)
        {
            var request = new LetterDto
            {
                Identifier = recipient,
                PdfFileBase64 = content,
                Subject = subject,
                Name = name
            };

            try
            {
                var canReceiveDigitalMailRequest = new CanReceiveDigitalPostDto
                {
                    Identifier = recipient
                };

                var canReceiveDigitalMail = await _client.CanReceiveDigitalMail(canReceiveDigitalMailRequest);
                request.CanReceiveDigitalMail = canReceiveDigitalMail;

                if (canReceiveDigitalMail != true)
                {
                    await TryAddAddressInformationToRequest(recipient, request);
                }

                var response = await _client.SendDigitalPost(request);
                var sentAs = GetActor(response.SentAs);

                if (!string.IsNullOrEmpty(response.Error))
                {
                    return new NotificationSentReceipt
                    {
                        IsSent = false,
                        SentAs = sentAs,
                        Errors = new List<string>{response.Error}
                    };
                }

                if (sentAs == NotificationSentAs.UNKNOWN)
                {
                    _logger.LogWarning("NotificationQueue with Id '{Id}' was successfully sent, but it was not possible to determine the provider. RemotePrint returned the following value as provider: '{provider}'. Value may be updated when checking delivering status.", queueId, response.SentAs);
                }

                if (string.IsNullOrEmpty(response.MessageId))
                {
                    _logger.LogWarning("NotificationQueue with Id '{Id}' was successfully sent, but no MessageId was provided in the response. NotificationQueue will be considered successfully sent when updating delivering status", queueId);
                }

                return new NotificationSentReceipt
                {
                    IsSent = true,
                    MessageId = response.MessageId,
                    SentAs = sentAs,
                    DeliveryStatus = NotificationDeliveryStatus.AWAITING
                };
            }
            catch (Exception ex)
            {
                return new NotificationSentReceipt
                {
                    IsSent = false,
                    SentAs = NotificationSentAs.UNKNOWN,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private async Task TryAddAddressInformationToRequest(string recipient, LetterDto request)
        {
            AddressInformation addressInformation = null;
            if (recipient.Length == 10)
            {
                addressInformation = await _cprInformationService.GetMailAddressInformation(recipient);
            }
            else if (recipient.Length == 8)
            {
                addressInformation = await _cvrInformationService.GetMailAddressInformation(recipient);
            }
            else
            {
                _logger.LogWarning("Invalid recipient identifier, unable to add address information. Identifier must be a CVR of 8 chars or CPR of 10 chars.");
                return;
            }

            if (addressInformation != null)
            {
                request.Name ??= addressInformation.Name;
                request.AddressInformation = new AddressInformationDto
                {
                    StreetName = addressInformation.StreetName,
                    StreetBuilding = addressInformation.StreetBuilding,
                    Floor = addressInformation.Floor,
                    Suite = addressInformation.Suite,
                    PostCode = addressInformation.PostalCode,
                    City = addressInformation.MailDistrict
                };
            }
            else
            {
                _logger.LogWarning("Unable to get address information for recipient with identifier {Identifier} and will thus not be added to the request.", AnonymiseIdentifier(recipient));
            }
        }

        private DeliveryStatus MapStatusResponseDtoToNotificationStatus(PkoStatusResponseDto responseDto)
        {
            var error = responseDto.PkoStatus.Error?.Message;
            var errors = string.IsNullOrEmpty(error) ? new List<string>() : new List<string> { error };
            var sentAs = GetActor(responseDto.Actor?.Name);
            var messageId = sentAs == NotificationSentAs.PHYSICAL_LETTER ? responseDto.PkoStatus?.DispatchIdentifier : responseDto.PkoStatus?.MessageUUID;
            return new DeliveryStatus 
            {
                Status = GetDeliveryStatus(responseDto.Action?.Id),
                MessageId = messageId,
                SentAs = sentAs,
                TransactionTime = responseDto.PkoStatus?.TransactionTime ?? DateTimeOffset.Now,
                Errors = errors
            };
        }

        private string AnonymiseIdentifier(string identifier)
        {
            if (identifier.Length == 8)
            {
                // CVR not secret
                return $"cvr = {identifier}";
            }

            // Mask last 4 digits of CPR
            var maskedCpr = identifier.Substring(0, Math.Min(6, identifier.Length)) + "-xxxx";

            return $"cpr = {maskedCpr}";
        }

        private NotificationDeliveryStatus GetDeliveryStatus(string actionId)
        {
            if (ResponseActions.SuccessfulActions.Contains(actionId))
            {
                return NotificationDeliveryStatus.SUCCESSFUL;
            }

            if (ResponseActions.FailedActions.Contains(actionId))
            {
                return NotificationDeliveryStatus.FAILED;
            }

            if (ResponseActions.AwaitingActions.Contains(actionId))
            {
                return NotificationDeliveryStatus.AWAITING;
            }

            return NotificationDeliveryStatus.UNKNOWN;
        }

        private NotificationSentAs GetActor(string sentAs)
        {
            if (ResponseActors.DigitalPostActors.Contains(sentAs))
            {
                return NotificationSentAs.DIGITAL_LETTER;
            }

            if (ResponseActors.RemotePrintActors.Contains(sentAs))
            {
                return NotificationSentAs.PHYSICAL_LETTER;
            }

            return NotificationSentAs.UNKNOWN;
        }
    }
}