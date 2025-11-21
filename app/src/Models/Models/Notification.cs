using Agora.Models.Common;
using System.Collections.Generic;

namespace Agora.Models.Models
{
    public class Notification : AuditableModel
    {
        public bool IsSentToQueue { get; set; }

        public int? HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }

        public int NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }

        public NotificationQueue NotificationQueue { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<EventMapping> EventMappings { get; set; } = new List<EventMapping>();
    }
}
