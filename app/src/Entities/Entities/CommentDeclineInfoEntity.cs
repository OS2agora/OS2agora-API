using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class CommentDeclineInfoEntity : AuditableEntity
    {
        public string DeclineReason { get; set; }
        public string DeclinerInitials { get; set; }
    }
}