using Agora.Models.Common;
using System.Collections.Generic;

namespace Agora.Models.Models
{
    public class NotificationContentSpecification : AuditableModel
    {
        public int? HeaderContentId { get; set; }
        public NotificationContent HeaderContent { get; set; }
        public int? BodyContentId { get; set; }
        public NotificationContent BodyContent { get; set; }
        public int? FooterContentId { get; set; }
        public NotificationContent FooterContent { get; set; }
        public int? SubjectContentId { get; set; }
        public NotificationContent SubjectContent { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }

        public static List<string> DefaultIncludes => new List<string>
        {
            nameof(SubjectContent),
            $"{nameof(SubjectContent)}.{nameof(NotificationContent.NotificationContentType)}",
            nameof(HeaderContent),
            $"{nameof(HeaderContent)}.{nameof(NotificationContent.NotificationContentType)}",
            nameof(BodyContent),
            $"{nameof(BodyContent)}.{nameof(NotificationContent.NotificationContentType)}",
            nameof(FooterContent),
            $"{nameof(FooterContent)}.{nameof(NotificationContent.NotificationContentType)}",
            nameof(NotificationType)
        };
    }
}