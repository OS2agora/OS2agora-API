using System;
using System.Collections.Generic;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Enums;

namespace BallerupKommune.Models.Models
{
    public class NotificationQueue : AuditableModel
    {
        public string Content { get; set; }
        public string[] ErrorTexts { get; set; }
        public string Subject { get; set; }
        public bool IsSend { get; set; }
        public string RecipientAddress { get; set; }
        public int RetryCount { get; set; }
        public DateTime? SuccessfullSendDate { get; set; }
        public NotificationMessageChannel MessageChannel { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
