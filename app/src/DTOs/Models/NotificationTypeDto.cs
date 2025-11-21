using Agora.DTOs.Common;
using NotificationTypeEnum = Agora.DTOs.Enums.NotificationType;

namespace Agora.DTOs.Models
{
    public class NotificationTypeDto : AuditableDto<NotificationTypeDto>
    {
        public string Name { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
    }
}