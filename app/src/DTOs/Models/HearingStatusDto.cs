using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class HearingStatusDto : AuditableDto<HearingStatusDto>
    {
        public Enums.HearingStatus Status { get; set; }
        public string Name { get; set; }

        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}