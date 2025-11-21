using Agora.Models.Models.Records;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.Files.Excel;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = Agora.Models.Models.Files.File;

namespace Agora.DAOs.Files.Excel
{
    public class ExcelService : IExcelService
    {
        private readonly IExcelTheme _excelTheme;

        public ExcelService(IExcelTheme excelTheme)
        {
            _excelTheme = excelTheme;
        }

        public byte[] CreateHearingReport(IEnumerable<CommentRecord> commentRecords)
        {
            var form = _excelTheme.GetHearingReportForm(commentRecords);
            return GenerateExcelFileFromForm(form);
        }

        public byte[] CreateHearingResponseReport(IEnumerable<CommentRecord> commentRecords)
        {
            var form = _excelTheme.GetHearingResponseReportForm(commentRecords);
            return GenerateExcelFileFromForm(form);
        }

        public byte[] CreateHearingUserReport(IEnumerable<UserRecord> userRecords)
        {
            var form = _excelTheme.GetHearingUserReportForm(userRecords);
            return GenerateExcelFileFromForm(form);
        }

        public Dictionary<string, List<string>> ParseExcel(File file, List<string> headers)
        {
            var extension = file.Extension;
            if (extension == null || extension.ToLowerInvariant() != ".xlsx")
            {
                throw new GeneralException("File must have the .xlsx extension");
            }

            var result = new Dictionary<string, List<string>>();
            foreach (var h in headers)
            {
                result[h] = new List<string>();
            }

            if (file.Content == null || file.Content.Length == 0)
            {
                return result;
            }

            try
            {
                using var ms = new MemoryStream(file.Content, writable: false);
                using var wb = new XLWorkbook(ms, XLEventTracking.Disabled);

                foreach (var sheet in wb.Worksheets)
                {
                    ParseSheetData(sheet, result, headers);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidFileContentException($"Failed to read {file.Extension} file '{file.Name}'. ", ex);
            }

        }

        private void ParseSheetData(IXLWorksheet sheet, Dictionary<string, List<string>> data, List<string> headers)
        {
            var dataRange = sheet.RangeUsed();
            if (dataRange is null)
            {
                return;
            }

            var headerRow = dataRange.FirstRow();
            var columns = GetColumnIndices(headerRow, headers);

            if (columns.Count < 1)
            {
                return;
            }

            foreach (var row in dataRange.RowsUsed().Skip(1))
            {
                if (row.IsEmpty())
                {
                    continue;
                }

                foreach (var column in columns)
                {
                    var header = column.Key;
                    var index = column.Value;

                    var cell = row.Cell(index);
                    var text = cell.GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }
                        
                    data[header].Add(text);
                }
            }
        }

        private Dictionary<string, int> GetColumnIndices(IXLRangeRow headerRow, List<string> headers)
        {
            var headerIndices = new Dictionary<string, int>();
            foreach (var cell in headerRow.CellsUsed())
            {
                var key = cell.GetString()?.Trim();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                if (headers.Contains(key))
                {
                    headerIndices[key] = cell.Address.ColumnNumber;
                }
            }

            return headerIndices;
        }

        private byte[] GenerateExcelFileFromForm(IExcelForm form)
        {
            using var memoryStream = new MemoryStream();
            using var workbook = form.GenerateContent();

            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }
}