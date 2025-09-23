using System.Collections.Generic;

namespace BallerupKommune.Models.Models.Records
{
    public class CommentRecord
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string OnBehalfOf { get; set; }
        public CommentDeclineInfo CommentDeclineInfo { get; set; }
        public string HearingOwnerDisplayName { get; set; }
        public List<string> FileNames { get; set; }
        public List<string> AnswersToComment { get; set; }
    }
}