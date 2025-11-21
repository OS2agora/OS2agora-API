using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class NotificationContent : AuditableModel
    {
        public string TextContent { get; set; }

        public int NotificationContentTypeId { get; set; }
        public NotificationContentType NotificationContentType { get; set; }

        public NotificationContentSpecification SubjectForSpecification { get; set; }
        public NotificationContentSpecification HeaderForSpecification { get; set; }
        public NotificationContentSpecification BodyForSpecification { get; set; }
        public NotificationContentSpecification FooterForSpecification { get; set; }

    }
}