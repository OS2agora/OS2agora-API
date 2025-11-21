using System.Collections.Generic;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class NotificationContentResult
    {
        public List<string> Content { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
    }
}