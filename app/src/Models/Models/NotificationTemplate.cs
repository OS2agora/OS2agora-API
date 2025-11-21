using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class NotificationTemplate : AuditableModel
    {
        public string Name { get; set; }
        public string TextContent { get; set; }

        public int NotificationContentTypeId { get; set; }
        public NotificationContentType NotificationContentType { get; set; }

        public ICollection<NotificationType> HeaderTemplateSpecifications { get; set; } = new List<NotificationType>();
        public ICollection<NotificationType> BodyTemplateSpecifications { get; set; } = new List<NotificationType>();
        public ICollection<NotificationType> FooterTemplateSpecifications { get; set; } = new List<NotificationType>();
        public ICollection<NotificationType> SubjectTemplateSpecifications { get; set; } = new List<NotificationType>();
    }
}
