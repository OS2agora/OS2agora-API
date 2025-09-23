using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class NotificationTypeEntity : AuditableEntity
    {
        public string Name { get; set; }

        public Enums.NotificationType Type { get; set; }

        public Enums.NotificationFrequency Frequency { get; set; }

        // One-to-one relationship with NotificationTemplate
        public int? NotificationTemplateId { get; set; }
        public NotificationTemplateEntity NotificationTemplate { get; set; }

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    }
}
