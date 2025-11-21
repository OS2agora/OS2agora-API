using System.Collections.Generic;
using Agora.Entities.Common;
using Agora.Entities.Enums;

namespace Agora.Entities.Entities
{
    public class NotificationTypeEntity : AuditableEntity
    {
        public string Name { get; set; }

        public NotificationType Type { get; set; }

        // Many-to-one relationship with NotificationTemplate
        public int HeaderTemplateId { get; set; }
        public NotificationTemplateEntity HeaderTemplate { get; set; }
        public int BodyTemplateId { get; set; }
        public NotificationTemplateEntity BodyTemplate { get; set; }
        public int FooterTemplateId { get; set; }
        public NotificationTemplateEntity FooterTemplate { get; set; }
        public int SubjectTemplateId { get; set; }
        public NotificationTemplateEntity SubjectTemplate { get; set; }

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();

        // One-to-many relationship with NotificationContentSpecification
        public ICollection<NotificationContentSpecificationEntity> NotificationContentSpecifications { get; set; } = new List<NotificationContentSpecificationEntity>();

        // One-to-many relationship with Event
        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
    }
}
