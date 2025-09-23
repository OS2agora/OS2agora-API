using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class ConsentDto : AuditableDto<ConsentDto>
    {
        public BaseDto<GlobalContentDto> GlobalContent { get; set; }

        public BaseDto<CommentDto> Comment { get; set; }
    }
}