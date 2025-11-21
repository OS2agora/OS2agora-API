using Agora.Entities.Common;
using Agora.Entities.Enums;
using System;

namespace Agora.Entities.Entities
{
    public class NotificationQueueEntity : AuditableEntity
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

        // One-to-one relationship with Notification 
        public int NotificationId { get; set; }
        public NotificationEntity Notification { get; set; }
    }
}