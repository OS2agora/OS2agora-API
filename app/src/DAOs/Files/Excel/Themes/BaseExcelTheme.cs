using Agora.DAOs.Files.Excel.Forms.Default;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files.Excel;
using System.Collections.Generic;

namespace Agora.DAOs.Files.Excel.Themes
{
    public class BaseExcelTheme : IExcelTheme
    {
        public virtual IExcelForm GetHearingReportForm(IEnumerable<CommentRecord> commentRecords) =>
            new HearingReportExcelForm
            {
                CommentRecords = commentRecords
            };

        public virtual IExcelForm GetHearingUserReportForm(IEnumerable<UserRecord> userRecords) =>
            new HearingUserReportExcelForm
            {
                UserRecords = userRecords
            };

        public IExcelForm GetHearingResponseReportForm(IEnumerable<CommentRecord> commentRecords) =>
            new HearingResponseReportExcelForm()
            {
                CommentRecords = commentRecords
            };
    }
}
