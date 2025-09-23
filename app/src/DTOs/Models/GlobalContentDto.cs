using BallerupKommune.DTOs.Common;
using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models
{
    public class GlobalContentDto : AuditableDto<GlobalContentDto>
    {
        public int Version { get; set; }

        public string Content { get; set; }

        public BaseDto<GlobalContentTypeDto> GlobalContentType { get; set; }

        public ICollection<ConsentDto> Consents { get; set; } = new List<ConsentDto>();
    }
}