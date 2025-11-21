using System.Collections.Generic;
using Agora.DTOs.Enums;

namespace Agora.DTOs.Models.Multipart
{
    public class MultiPartCreateCommentDto
    {
        public CommentType CommentType { get; set; }
        public string Content { get; set; }
        public string OnBehalfOf { get; set; }
        public int? CommentParrentId { get; set; }
        public List<FileOperationDto> FileOperations { get; set; }
    }
}