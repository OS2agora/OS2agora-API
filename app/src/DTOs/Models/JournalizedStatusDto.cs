using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class JournalizedStatusDto : AuditableDto<JournalizedStatusDto>
    {
        public Enums.JournalizedStatus Status { get; set; }
        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}
