using Agora.Models.Models.Records;
using System.Collections.Generic;

namespace Agora.Operations.Common.Interfaces.Files.Pdf
{
    public interface IPdfService
    {
        byte[] CreateHearingPdf(HearingRecord hearingRecord);

        byte[] CreateConclusionPdf(HearingRecord hearingRecord);

        byte[] CreateCommentPdf(CommentRecord commentRecord, HearingRecord hearingRecord);

        byte[] CreateHearingReport(HearingRecord hearingRecord);

        byte[] CreateFullHearingReport(HearingRecord hearingRecord);

        byte[] CreateTextPdf(List<string> content, string title, string subject);
    }
}