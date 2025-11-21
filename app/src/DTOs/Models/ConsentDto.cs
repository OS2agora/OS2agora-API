using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class ConsentDto : AuditableDto<ConsentDto>
    {
        public BaseDto<GlobalContentDto> GlobalContent { get; set; }

        public BaseDto<CommentDto> Comment { get; set; }
    }
}