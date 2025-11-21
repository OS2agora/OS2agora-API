using System;
using System.Collections.Generic;

namespace Agora.Models.Models.Records
{
    public class HearingBaseData
    {
        public int Id { get; set; }
        public string SubjectArea { get; set; }
        public string CityArea { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline {  get; set; }
        public bool ClosedHearing { get; set; }
        public string HearingType { get; set; }
        public string EsdhNumber { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string BodyText { get; set; }
        public string? Conclusion { get; set; }
        public DateTime? ConclusionCreatedDate { get; set; }
        public FileRecord Image { get; set; }
    }

    public class CommentStats
    {
        public int TotalResponses { get; set; }
        public int CitizenResponses { get; set; }
        public int CompanyResponses { get; set; }
        public int EmployeeResponses { get; set; }
        public int OnBehalfOfResponses { get; set; }
    }

    public class HearingRecord
    {
        public HearingBaseData BaseData { get; set; }
        public CommentStats CommentStats { get; set; }
        public List<FileRecord> Attachments { get; set; }
        public List<FileRecord> ConclusionAttachments { get; set; }
        public List<CommentRecord> CommentRecords { get; set; }
    }
}
