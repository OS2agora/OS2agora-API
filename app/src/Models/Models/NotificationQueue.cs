using Agora.Models.Common;
using Agora.Models.Enums;
using System;

namespace Agora.Models.Models
{
    public class NotificationQueue : AuditableModel
    {
        public string Content { get; set; }

        public string[] ErrorTexts { get; set; }

        public string Subject { get; set; }

        public bool IsSent { get; set; }

        public string RecipientAddress { get; set; }

        public int RetryCount { get; set; }

        public DateTime? SuccessfulSentDate { get; set; }

        public DateTime? SuccessfulDeliveryDate { get; set; }

        public string MessageId { get; set; }

        public NotificationDeliveryStatus DeliveryStatus { get; set; }

        public NotificationSentAs SentAs { get; set; }

        public NotificationMessageChannel MessageChannel { get; set; }

        public int NotificationId { get; set; }

        public Notification Notification { get; set; }
    }
}
