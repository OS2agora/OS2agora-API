using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class CommentDeclineInfo : AuditableModel
    {
        public string DeclineReason { get; set; }
        public string DeclinerInitials { get; set; }
    }
}
