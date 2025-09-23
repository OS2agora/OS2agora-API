using System;
using System.Collections.Generic;
using BallerupKommune.Entities.Common;
using BallerupKommune.Entities.Enums;

namespace BallerupKommune.Entities.Entities
{
    public class NotificationQueueEntity : AuditableEntity
    {
        public string Content { get; set; }

        public string[] ErrorTexts { get; set; }

        public string Subject { get; set; }

        public bool IsSend { get; set; }

        public string RecipientAddress { get; set; }

        public int RetryCount { get; set; }

        public DateTime? SuccessfullSendDate { get; set; }

        public NotificationMessageChannel MessageChannel { get; set; }

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    }
}