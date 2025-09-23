using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class NotificationTemplate : AuditableModel
    {
        public string NotificationTemplateText { get; set; }
        public string SubjectTemplate { get; set; }

        public int NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
