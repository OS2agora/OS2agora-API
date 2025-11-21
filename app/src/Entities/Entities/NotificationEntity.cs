using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class NotificationEntity : AuditableEntity
    {
        public bool IsSentToQueue { get; set; }

        // Many-to-one relationship with Hearing
        public int? HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        // Many-to-one relationship with Comment
        public int? CommentId { get; set; }
        public CommentEntity Comment { get; set; }

        // Many-to-one relationship with NotificationType
        public int NotificationTypeId { get; set; }
        public NotificationTypeEntity NotificationType { get; set; }

        // One-to-one relationship with NotificationQueue 
        public NotificationQueueEntity NotificationQueue { get; set; }

        // Many-to-one relationship with User
        public int? UserId { get; set; }
        public UserEntity User { get; set; }

        // Many-to-one relationship with Company
        public int? CompanyId { get; set; }
        public CompanyEntity Company { get; set; }

        // One-to-many relationship with EventMapping
        public ICollection<EventMappingEntity> EventMappings { get; set; } = new List<EventMappingEntity>();
    }
}
