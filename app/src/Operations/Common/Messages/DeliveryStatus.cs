using System;
using System.Collections.Generic;
using Agora.Models.Enums;

namespace Agora.Operations.Common.Messages
{
    public class DeliveryStatus
    {
        public string MessageId { get; set; }
        public NotificationSentAs SentAs { get; set; }
        public NotificationDeliveryStatus Status { get; set; }
        public DateTimeOffset TransactionTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}