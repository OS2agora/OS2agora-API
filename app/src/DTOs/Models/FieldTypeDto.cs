using BallerupKommune.DTOs.Common;
using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models
{
    public class FieldTypeDto : AuditableDto<FieldTypeDto>
    {
        public Enums.FieldType FieldType { get; set; }

        public ICollection<FieldDto> Fields { get; set; } = new List<FieldDto>();

        public ICollection<FieldTypeSpecificationDto> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecificationDto>();
    }
}