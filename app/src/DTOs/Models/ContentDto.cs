using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class ContentDto : AuditableDto<ContentDto>
    {
        public string TextContent { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileContentType { get; set; }

        public BaseDto<CommentDto> Comment { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<FieldDto> Field { get; set; }

        public BaseDto<ContentTypeDto> ContentType { get; set; }
    }
}