using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class EventMappingEntity : AuditableEntity
    {
        // Many-to-one relationship with Event
        public int EventId { get; set; }
        public EventEntity Event { get; set; }

        // Many-to-one relationship with Notification
        public int NotificationId { get; set; }
        public NotificationEntity Notification { get; set; }
    }
}