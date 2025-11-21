using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class Consent : AuditableModel
    {
        public int GlobalContentId { get; set; }
        public GlobalContent GlobalContent { get; set; }

        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}