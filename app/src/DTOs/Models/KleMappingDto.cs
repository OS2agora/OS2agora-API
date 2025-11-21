using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class KleMappingDto : AuditableDto<KleMappingDto>
    {
        public BaseDto<KleHierarchyDto> KleHierarchy { get; set; }

        public BaseDto<HearingTypeDto> HearingType { get; set; }
    }
}