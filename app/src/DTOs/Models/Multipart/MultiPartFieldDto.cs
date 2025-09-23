using System.Collections.Generic;
using BallerupKommune.DTOs.Enums;

namespace BallerupKommune.DTOs.Models.Multipart
{
    public class MultiPartFieldDto
    {
        public FieldType FieldType { get; set; }
        public string Content { get; set; }
        public bool IsEmptyContent { get; set; }
        public List<FileOperationDto> FileOperations { get; set; }
    }
}