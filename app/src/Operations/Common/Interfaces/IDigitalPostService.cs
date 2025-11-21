using System.Collections.Generic;
using Agora.Operations.Common.Messages;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces
{
    public interface IDigitalPostService
    {
        Task<NotificationSentReceipt> SendMessage(int notificationQueueId, string subject, string content, string recipient,
            string name = null, string address = null, string postalCode = null, string city = null);

        Task<List<DeliveryStatus>> GetMessageDeliveryStatus();
    }
}