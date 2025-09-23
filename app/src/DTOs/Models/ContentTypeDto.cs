using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class ContentTypeDto : AuditableDto<ContentTypeDto>
    {
        public Enums.ContentType ContentType { get; set; }

        public ICollection<ContentDto> Contents { get; set; } = new List<ContentDto>();

        public ICollection<FieldTypeSpecificationDto> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecificationDto>();
    }
}