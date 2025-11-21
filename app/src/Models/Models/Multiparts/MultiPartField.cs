using System.Collections.Generic;

namespace Agora.Models.Models.Multiparts
{
    public class MultiPartField
    {
        public Enums.FieldType FieldType { get; set; }
        public string Content { get; set; }
        public bool IsEmptyContent { get; set; }
        public List<FileOperation> FileOperations { get; set; }
    }
}