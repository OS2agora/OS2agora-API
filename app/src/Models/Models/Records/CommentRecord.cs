using System;
using System.Collections.Generic;
using UserCapacityEnum = Agora.Models.Enums.UserCapacity;
using CommentStatusEnum = Agora.Models.Enums.CommentStatus;
using CommentTypeEnum = Agora.Models.Enums.CommentType;

namespace Agora.Models.Models.Records
{

    public class CommentRecord
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsDeleted { get; set; }
        public CommentStatusEnum Status { get; set; }
        public CommentTypeEnum Type { get; set; }
        public string CommentText { get; set; }
        public UserRecord Responder {  get; set; }
        public CompanyRecord Company { get; set; }
        public string ResponderName { get; set; }
        public UserCapacityEnum ResponderCapacity { get; set; }
        public string OnBehalfOf { get; set; }
        public string HearingOwnerDisplayName { get; set; }
        public CommentDeclineInfo CommentDeclineInfo { get; set; }
        public DateTime Created { get; set; }
        public List<string> FileNames { get; set; }
        public List<string> AnswersToComment { get; set; }
        public List<FileRecord> Files { get; set; }
    }


}