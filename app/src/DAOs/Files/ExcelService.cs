using BallerupKommune.Models.Models.Records;
using BallerupKommune.Operations.Common.Interfaces;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BallerupKommune.DAOs.Files
{
    public class ExcelService : IExcelService
    {
        public byte[] CreateHearingReport(List<CommentRecord> commentRecords)
        {
            using var memoryStream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Høringssvar");

            var dataTable = CreateDataTable(commentRecords);

            worksheet.Cell("A1").InsertTable(dataTable.AsEnumerable());
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }

        private DataTable CreateDataTable(List<CommentRecord> commentRecords)
        {
            var table = new DataTable();
            table.Columns.Add("Høringssvar #", typeof(int));
            table.Columns.Add("Høringssvar", typeof(string));
            table.Columns.Add("På vegne af", typeof(string));
            table.Columns.Add("Tilknyttede filer", typeof(string));
            table.Columns.Add("Begrundelse for afvisning", typeof(string));
            table.Columns.Add("Kommentar til høringssvar", typeof(string));

            foreach (var commentRecord in commentRecords)
            {
                table.Rows.Add(
                    commentRecord.Id,
                    commentRecord.Comment,
                    commentRecord.OnBehalfOf,
                    string.Join(", ", commentRecord.FileNames),
                    GetDeclineReasonString(commentRecord),
                    GetCommentAnswersAsString(commentRecord));
            }

            return table;
        }

        private string GetDeclineReasonString(CommentRecord commentRecord)
        {
            if (string.IsNullOrEmpty(commentRecord?.CommentDeclineInfo?.DeclineReason))
            {
                return "";
            }

            return "Skrevet af " + commentRecord?.CommentDeclineInfo?.DeclinerInitials + ": " + commentRecord.CommentDeclineInfo.DeclineReason;
        }

        private string GetCommentAnswersAsString(CommentRecord commentRecord)
        {
            var commentAnswersString = "";
            foreach (var answer in commentRecord.AnswersToComment)
            {
                commentAnswersString += "Skrevet af " + commentRecord.HearingOwnerDisplayName + ": " + answer;
            }

            return commentAnswersString;
        }
    }
}