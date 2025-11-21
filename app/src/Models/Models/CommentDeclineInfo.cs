using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class CommentDeclineInfo : AuditableModel
    {
        public string DeclineReason { get; set; }
        public string DeclinerInitials { get; set; }
    }
}
