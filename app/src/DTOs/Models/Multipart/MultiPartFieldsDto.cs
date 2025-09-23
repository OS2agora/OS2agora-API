using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models.Multipart
{
    public class MultiPartFieldsDto
    {
        public IEnumerable<MultiPartFieldDto> Fields { get; set; }
    }
}