using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class NotificationContentType : AuditableModel
    {
        public Enums.NotificationContentType Type { get; set; }
        
        public bool CanEdit { get; set; }

        public ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();

        public ICollection<NotificationContent> NotificationContents { get; set; } = new List<NotificationContent>();
    }
}