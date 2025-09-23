using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class NotificationEntity : AuditableEntity
    {
        public bool IsSendToQueue { get; set; }

        // Many-to-one relationship with Hearing
        public int HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        public int? CommentId { get; set; }
        public CommentEntity Comment { get; set; }

        // Many-to-one relationship with NotificationType
        public int NotificationTypeId { get; set; }
        public NotificationTypeEntity NotificationType { get; set; }

        // Many-to-one relationship with NotificationQueue 
        public int? NotificationQueueId { get; set; }
        public NotificationQueueEntity NotificationQueue { get; set; }

        // Many-to-one relationship with User
        public int? UserId { get; set; }
        public UserEntity User { get; set; }

        public int? CompanyId { get; set; }
        public CompanyEntity Company { get; set; }
    }
}
