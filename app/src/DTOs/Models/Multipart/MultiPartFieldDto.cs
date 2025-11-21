using System.Collections.Generic;
using Agora.DTOs.Enums;

namespace Agora.DTOs.Models.Multipart
{
    public class MultiPartFieldDto
    {
        public FieldType FieldType { get; set; }
        public string Content { get; set; }
        public bool IsEmptyContent { get; set; }
        public List<FileOperationDto> FileOperations { get; set; }
    }
}