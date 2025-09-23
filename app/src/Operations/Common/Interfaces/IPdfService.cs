using BallerupKommune.Models.Models.Records;
using System;
using System.Collections.Generic;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IPdfService
    {
        byte[] CreateHearingPdf(string hearingTitle, string hearingSummary, string hearingBodyInformation,
            string esdhNumber, string hearingType, string subjectArea, DateTime startDate, DateTime deadline);

        byte[] CreateConclusionPdf(string esdhNumber, string hearingSummary, string hearingType, string subjectArea,
            string conclusionText, DateTime conclusionDate);

        byte[] CreateCommentPdf(string hearingTitle, string commentWriterName, string commentText, string esdhNumber, string hearingType,
            string subjectArea, DateTime created);

        byte[] CreateHearingReport(string hearingTitle, string esdhNumber, List<CommentRecord> commentRecords);

        byte[] CreateTextPdf(List<string> content, string title, string subject);
    }
}