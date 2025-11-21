using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class NotificationDto : AuditableDto<NotificationDto>
    {
        public bool IsSentToQueue { get; set; }
        public BaseDto<HearingDto> Hearing { get; set; }
        public BaseDto<UserDto> User { get; set; }
        public BaseDto<CompanyDto> Company { get; set; }
        public BaseDto<NotificationQueueDto> NotificationQueue { get; set; }
        public BaseDto<NotificationTypeDto> NotificationType { get; set; }
    }
}