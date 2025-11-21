using Agora.DAOs.Messages.EBoks;
using Agora.Models.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.DAOs.Messages
{
    public class EBoksService : IDigitalPostService
    {
        private readonly EBoksClient _eBoksClient;

        public EBoksService(EBoksClient eBoksClient)
        {
            _eBoksClient = eBoksClient;
        }

        public Task<List<DeliveryStatus>> GetMessageDeliveryStatus()
        {
            return Task.FromResult(new List<DeliveryStatus>());
        }

        public async Task<NotificationSentReceipt> SendMessage(int notificationQueueId, string subject, string content, string recipient, 
            string name = null, 
            string address = null,
            string postalCode = null, 
            string city = null)
        {
            try
            {
                var receipt = await _eBoksClient.Send(subject, recipient, subject, content);

                return new NotificationSentReceipt
                {
                    MessageId = notificationQueueId.ToString(),
                    IsSent = receipt.IsSent, 
                    SentAs = NotificationSentAs.DIGITAL_LETTER,
                    DeliveryStatus = receipt.IsSent ? NotificationDeliveryStatus.SUCCESSFUL : NotificationDeliveryStatus.AWAITING,
                    Errors = new List<string> { receipt.Error }
                };
            }
            catch (Exception ex)
            {
                return new NotificationSentReceipt
                {
                    IsSent = false,
                    SentAs = NotificationSentAs.DIGITAL_LETTER,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}