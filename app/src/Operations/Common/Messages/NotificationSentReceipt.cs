using System.Collections.Generic;
using Agora.Models.Enums;

namespace Agora.Operations.Common.Messages
{
    public class NotificationSentReceipt
    {
        public string MessageId { get; set; }
        public bool IsSent { get; set; }
        public NotificationSentAs SentAs { get; set; }
        public NotificationDeliveryStatus DeliveryStatus { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}