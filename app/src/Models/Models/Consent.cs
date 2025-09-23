using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class Consent : AuditableModel
    {
        public int GlobalContentId { get; set; }
        public GlobalContent GlobalContent { get; set; }

        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}