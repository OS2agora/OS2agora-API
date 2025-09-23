using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class KleMappingDto : AuditableDto<KleMappingDto>
    {
        public BaseDto<KleHierarchyDto> KleHierarchy { get; set; }

        public BaseDto<HearingTypeDto> HearingType { get; set; }
    }
}