using Agora.Models.Models.Files;
using Agora.Models.Models.Records;
using System.Collections.Generic;

namespace Agora.Operations.Common.Interfaces.Files.Excel
{
    public interface IExcelService
    {
        Dictionary<string, List<string>> ParseExcel(File file, List<string> headers);
        byte[] CreateHearingReport(IEnumerable<CommentRecord> commentRecords);

        byte[] CreateHearingUserReport(IEnumerable<UserRecord> commentRecords);

        byte[] CreateHearingResponseReport(IEnumerable<CommentRecord> commentRecords);
    }
}