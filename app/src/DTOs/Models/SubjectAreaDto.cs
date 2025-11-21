using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class SubjectAreaDto : AuditableDto<SubjectAreaDto>
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}