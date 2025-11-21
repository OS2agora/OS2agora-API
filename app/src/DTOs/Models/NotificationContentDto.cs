using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class NotificationContentDto : AuditableDto<NotificationContentDto>
    {
        public string TextContent { get; set; }

        public BaseDto<NotificationContentTypeDto> NotificationContentType { get; set; }
    }
}