using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class NotificationContentSpecificationDto : AuditableDto<NotificationContentSpecificationDto>
    {
        public BaseDto<NotificationContentDto> HeaderContent { get; set; }
        public BaseDto<NotificationContentDto> BodyContent { get; set; }
        public BaseDto<NotificationContentDto> FooterContent { get; set; }
        public BaseDto<NotificationContentDto> SubjectContent { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<NotificationTypeDto> NotificationType { get; set; }
    }
}