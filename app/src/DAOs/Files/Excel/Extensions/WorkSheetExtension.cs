using ClosedXML.Excel;
using System.Data;

namespace Agora.DAOs.Files.Excel.Extensions
{
    public static class WorkSheetExtension
    {
        public static void AddTableToCell(this IXLWorksheet worksheet, DataTable table, int maxWidth = 80, string cell = "A1")
        {
            worksheet.Cell(cell).InsertTable(table, createTable: true);
            worksheet.Style.Alignment.WrapText = true;
            worksheet.Columns().AdjustToContents();

            foreach (var column in worksheet.ColumnsUsed())
            {
                if (column.Width > maxWidth)
                {
                    column.Width = maxWidth;
                }
            }
        }
    }
}
