using ClosedXML.Excel;

namespace Agora.Operations.Common.Interfaces.Files.Excel
{
    public interface IExcelForm
    {
        XLWorkbook GenerateContent();
    }
}
