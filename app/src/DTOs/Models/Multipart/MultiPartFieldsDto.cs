using System.Collections.Generic;

namespace Agora.DTOs.Models.Multipart
{
    public class MultiPartFieldsDto
    {
        public IEnumerable<MultiPartFieldDto> Fields { get; set; }
    }
}