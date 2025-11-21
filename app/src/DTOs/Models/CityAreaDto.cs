using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class CityAreaDto : AuditableDto<CityAreaDto>
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}