using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class EventMapping : AuditableModel
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
    }
}