using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class NotificationContentEntity : AuditableEntity
    {
        public string TextContent { get; set; }

        // Many-to-one relationship with NotificationContentType
        public int NotificationContentTypeId { get; set; }
        public NotificationContentTypeEntity NotificationContentType { get; set; }

        // Many-to-one relationship with NotificationContentSpecification
        public NotificationContentSpecificationEntity SubjectForSpecification { get; set; }
        public NotificationContentSpecificationEntity HeaderForSpecification { get; set; }
        public NotificationContentSpecificationEntity BodyForSpecification { get; set; }
        public NotificationContentSpecificationEntity FooterForSpecification { get; set; }

    }
}