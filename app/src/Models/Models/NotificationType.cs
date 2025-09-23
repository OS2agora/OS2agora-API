using System.Collections.Generic;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Enums;

namespace BallerupKommune.Models.Models
{
    public class NotificationType : AuditableModel
    {
        public string Name { get; set; }

        public Enums.NotificationType Type { get; set; }
        
        public NotificationFrequency Frequency { get; set; }

        public int? NotificationTemplateId { get; set; }
        public NotificationTemplate NotificationTemplate { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
