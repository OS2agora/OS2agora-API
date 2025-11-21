using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class NotificationTemplateEntity : AuditableEntity
    {
        public string Name { get; set; }
        public string TextContent { get; set; }

        // Many-to-one relationship with NotificationContentType
        public int NotificationContentTypeId { get; set; }
        public NotificationContentTypeEntity NotificationContentType { get; set; }

        // One-to-many relationship with NotificationTemplateSpecification
        public ICollection<NotificationTypeEntity> HeaderTemplateSpecifications { get; set; } = new List<NotificationTypeEntity>();
        public ICollection<NotificationTypeEntity> BodyTemplateSpecifications { get; set; } = new List<NotificationTypeEntity>();
        public ICollection<NotificationTypeEntity> FooterTemplateSpecifications { get; set; } = new List<NotificationTypeEntity>();
        public ICollection<NotificationTypeEntity> SubjectTemplateSpecifications { get; set; } = new List<NotificationTypeEntity>();
    }
}
