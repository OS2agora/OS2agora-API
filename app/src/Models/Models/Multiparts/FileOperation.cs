using Agora.Models.Enums;
using Agora.Models.Models.Files;

namespace Agora.Models.Models.Multiparts
{
    public class FileOperation
    {
        public File File { get; set; }
        public int ContentId { get; set; } // Is included in a DELETE operation and reference the Content to delete
        public FileOperationEnum Operation { get; set; }
        public bool MarkedByScanner { get; set; }
    }
}