using System.Collections.Generic;
using Agora.Entities.Common;
using Agora.Entities.Enums;

namespace Agora.Entities.Entities
{
    public class NotificationContentTypeEntity : AuditableEntity
    {
        public NotificationContentType Type { get; set; }

        public bool CanEdit { get; set; }

        // One-to-many relationship with NotificationTemplate
        public ICollection<NotificationTemplateEntity> NotificationTemplates { get; set; } = new List<NotificationTemplateEntity>();

        // One-to-many relationship with NotificationContent
        public ICollection<NotificationContentEntity> NotificationContents { get; set; } = new List<NotificationContentEntity>();
    }
}