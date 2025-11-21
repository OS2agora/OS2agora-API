using Agora.Models.Common;
using System.Collections.Generic;

namespace Agora.Models.Models
{
    public class NotificationType : AuditableModel
    {
        public string Name { get; set; }

        public Enums.NotificationType Type { get; set; }

        public int HeaderTemplateId { get; set; }
        public NotificationTemplate HeaderTemplate { get; set; }
        public int BodyTemplateId { get; set; }
        public NotificationTemplate BodyTemplate { get; set; }
        public int FooterTemplateId { get; set; }
        public NotificationTemplate FooterTemplate { get; set; }
        public int SubjectTemplateId { get; set; }
        public NotificationTemplate SubjectTemplate { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<NotificationContentSpecification> NotificationContentSpecifications { get; set; } = new List<NotificationContentSpecification>();

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
