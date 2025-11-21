using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class NotificationContentTypeDto : AuditableDto<NotificationContentTypeDto>
    {
        public Enums.NotificationContentType NotificationContentType { get; set; }

        public bool CanEdit { get; set; }
    }
}