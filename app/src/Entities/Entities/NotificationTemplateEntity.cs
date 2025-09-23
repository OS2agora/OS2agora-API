using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class NotificationTemplateEntity : AuditableEntity
    {
        public string NotificationTemplateText { get; set; }

        public string SubjectTemplate { get; set; }

        // One-to-one relationship with NotificationType
        public NotificationTypeEntity NotificationType { get; set; }
    }
}
