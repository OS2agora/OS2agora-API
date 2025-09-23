using BallerupKommune.DTOs.Enums;
using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models.Multipart
{
    public class MultiPartUpdateCommentDto
    {
        public CommentStatus CommentStatus { get; set; }
        public string Content { get; set; }
        public string OnBehalfOf { get; set; }
        public string CommentDeclineReason { get; set; }
        public List<FileOperationDto> FileOperations { get; set; }
    }
}