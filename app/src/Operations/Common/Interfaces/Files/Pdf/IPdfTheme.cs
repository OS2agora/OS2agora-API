using Agora.Models.Models.Records;
using System.Collections.Generic;

namespace Agora.Operations.Common.Interfaces.Files.Pdf
{
    public interface IPdfTheme
    {
        IPdfForm GetHearingForm(HearingRecord hearingRecord);
        IPdfForm GetConclusionForm(HearingRecord hearingRecord);
        IPdfForm GetCommentForm(CommentRecord commentRecord, HearingRecord hearingRecord);
        IPdfForm GetHearingReportForm(HearingRecord hearingRecord);
        IPdfForm GetFullHearingReportForm(HearingRecord hearingRecord);
        IPdfForm GetTextForm(List<string> content, string title, string subject);
    }
}
