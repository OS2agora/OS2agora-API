using BallerupKommune.Models.Models.Records;
using System.Collections.Generic;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IExcelService
    {
        byte[] CreateHearingReport(List<CommentRecord> commentRecords);
    }
}