using Agora.DAOs.Files.Excel.Extensions;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Agora.DAOs.Files.Excel.Forms.Default
{
    public class HearingReportExcelForm : BaseExcelForm
    {
        private static Dictionary<string, Type> Headers => new()
        {
            { "Høringssvar #", typeof(int) },
            { "Høringssvar", typeof(string) },
            { "På vegne af", typeof(string) },
            { "Tilknyttede filer", typeof(string) },
            { "Kommentar til høringssvar", typeof(string) }
        };

        public override XLWorkbook GenerateContent()
        {
            var hearingAnswerSheet = AddWorkSheet("Høringssvar");
            var table = GenerateTable(Headers);

            AddCommentDataToTable(table);

            hearingAnswerSheet.AddTableToCell(table);

            return Workbook;
        }

        private void AddCommentDataToTable(DataTable table)
        {
            foreach (var record in CommentRecords)
            {
                var attachments = string.Join(", ", record.FileNames);
                var answers = string.Join("--- ", record.AnswersToComment);
                table.Rows.Add(record.Id, record.CommentText, record.OnBehalfOf, attachments, answers);
            }
        }
    }
}
