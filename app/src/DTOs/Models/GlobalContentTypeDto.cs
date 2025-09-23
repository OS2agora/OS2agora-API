using BallerupKommune.DTOs.Common;
using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models
{
    public class GlobalContentTypeDto : AuditableDto<GlobalContentTypeDto>
    {
        public string Name { get; set; }

        public Enums.GlobalContentType GlobalContentType { get; set; }

        public ICollection<GlobalContentDto> GlobalContents { get; set; } = new List<GlobalContentDto>();
    }
}