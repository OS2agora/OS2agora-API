using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class NotificationContentSpecificationEntity : AuditableEntity
    {
        // Many (4) One-to-one relationship with NotificationContent
        public int? HeaderContentId { get; set; }
        public NotificationContentEntity HeaderContent { get; set; }
        public int? BodyContentId { get; set; }
        public NotificationContentEntity BodyContent { get; set; }
        public int? FooterContentId { get; set; }
        public NotificationContentEntity FooterContent { get; set; }
        public int? SubjectContentId { get; set; }
        public NotificationContentEntity SubjectContent { get; set; }

        // Many-to-one relationship with Hearing
        public int HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        // Many-to-one relationship with NotificationType
        public int NotificationTypeId { get; set; }
        public NotificationTypeEntity NotificationType { get; set; }
    }
}