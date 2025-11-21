using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class EventEntity : AuditableEntity
    {
        public bool IsSentInNotification { get; set; }

        public Enums.EventType Type { get; set; }

        // Many-to-one relationship with Hearing
        public int HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        // Many-to-one relationship with User
        public int? UserId { get; set; }
        public UserEntity User { get; set; }

        // Many-to-one relationship with NotificationType
        public int NotificationTypeId { get; set; }
        public NotificationTypeEntity NotificationType { get; set; }

        // One-to-many relationship with EventMapping
        public ICollection<EventMappingEntity> EventMappings { get; set; } = new List<EventMappingEntity>();
    }
}