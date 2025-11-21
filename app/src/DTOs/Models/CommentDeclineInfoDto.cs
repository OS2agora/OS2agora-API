using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class CommentDeclineInfoDto : AuditableDto<CommentDeclineInfoDto>
    {
        public string DeclineReason { get; set; }

        public string DeclinerInitials { get; set; }

    }
}
