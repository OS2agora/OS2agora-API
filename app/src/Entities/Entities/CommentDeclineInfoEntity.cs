using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class CommentDeclineInfoEntity : AuditableEntity
    {
        public string DeclineReason { get; set; }
        public string DeclinerInitials { get; set; }
    }
}
