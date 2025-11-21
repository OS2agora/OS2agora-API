using Agora.Models.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.DAOs.Messages
{
    public class DigitalPostStubService : IDigitalPostService
    {
        public Task<List<DeliveryStatus>> GetMessageDeliveryStatus()
        {
            return Task.FromResult(new List<DeliveryStatus>());
        }

        public Task<NotificationSentReceipt> SendMessage(int queueId, string subject, string content, string recipient, string name = null, string address = null,
            string postalCode = null, string city = null)
        {
            return Task.FromResult(new NotificationSentReceipt
            {
                IsSent = true, 
                MessageId = queueId.ToString(), 
                SentAs = NotificationSentAs.DIGITAL_LETTER,
                DeliveryStatus = NotificationDeliveryStatus.SUCCESSFUL
            });
        }
    }
}