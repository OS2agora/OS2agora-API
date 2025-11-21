using Agora.DTOs.Enums;
using Microsoft.AspNetCore.Http;

namespace Agora.DTOs.Models.Multipart
{
    public class FileOperationDto
    {
        public IFormFile File { get; set; }
        public int ContentId { get; set; } // Is included in a DELETE operation and reference the Content to delete
        public FileOperationEnum Operation { get; set; }
    }
}