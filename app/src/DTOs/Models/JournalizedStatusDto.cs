using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class JournalizedStatusDto : AuditableDto<JournalizedStatusDto>
    {
        public Enums.JournalizedStatus Status { get; set; }
        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}
