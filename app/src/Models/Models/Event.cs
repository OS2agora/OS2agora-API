using System.Collections.Generic;
using Agora.Models.Common;
using Agora.Models.Enums;

namespace Agora.Models.Models
{
    public class Event : AuditableModel
    {
        public bool IsSentInNotification { get; set; }

        public EventType Type { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }

        public ICollection<EventMapping> EventMappings { get; set; } = new List<EventMapping>();
    }
}