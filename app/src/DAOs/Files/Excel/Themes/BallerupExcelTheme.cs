using Agora.DAOs.Files.Excel.Forms.Default;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files.Excel;
using System.Collections.Generic;

namespace Agora.DAOs.Files.Excel.Themes
{
    public class BallerupExcelTheme : BaseExcelTheme
    {
        public override IExcelForm GetHearingReportForm(IEnumerable<CommentRecord> commentRecords) =>
            new BallerupHearingReportExcelForm
            {
                CommentRecords = commentRecords
            };
    }
}
