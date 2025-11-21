using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class KleHierarchyDto : AuditableDto<KleHierarchyDto>
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsActive { get; set; }

        public BaseDto<KleHierarchyDto> KleHierarchyParrent { get; set; }

        public ICollection<KleHierarchyDto> KleHierarchyChildren { get; set; } = new List<KleHierarchyDto>();

        public ICollection<KleMappingDto> KleMappings { get; set; } = new List<KleMappingDto>();

        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();
    }
}