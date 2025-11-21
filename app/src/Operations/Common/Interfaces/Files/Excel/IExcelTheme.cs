using Agora.Models.Models.Records;
using System.Collections.Generic;

namespace Agora.Operations.Common.Interfaces.Files.Excel
{
    public interface IExcelTheme
    {
        IExcelForm GetHearingReportForm(IEnumerable<CommentRecord> commentRecords);
        IExcelForm GetHearingUserReportForm(IEnumerable<UserRecord> userRecords);
        IExcelForm GetHearingResponseReportForm(IEnumerable<CommentRecord> commentRecords);
    }
}
