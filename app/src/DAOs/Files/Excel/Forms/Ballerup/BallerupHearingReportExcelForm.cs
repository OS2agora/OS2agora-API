using Agora.DAOs.Files.Excel.Extensions;
using Agora.Models.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Agora.DAOs.Files.Excel.Forms.Default
{
    public class BallerupHearingReportExcelForm : BaseExcelForm
    {
        private static Dictionary<string, Type> Headers => new()
        {
            { "Høringssvar #", typeof(int) },
            { "Høringssvar", typeof(string) },
            { "På vegne af", typeof(string) },
            { "Tilknyttede filer", typeof(string) },
            { "Begrundelse for afvisning", typeof(string) },
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
                var declineInfo = GetDeclineReasonString(record.CommentDeclineInfo);
                var answers = GetCommentAnswersAsString(record.AnswersToComment, record.HearingOwnerDisplayName);
                table.Rows.Add(record.Id, record.CommentText, record.OnBehalfOf, attachments, declineInfo, answers);
            }
        }

        private static string GetDeclineReasonString(CommentDeclineInfo declineInfo)
        {
            if (declineInfo == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder("Skrevet af ");

            sb.Append(!string.IsNullOrEmpty(declineInfo.DeclinerInitials)
                ? $"{declineInfo.DeclinerInitials}: "
                : "ukendt bruger: ");

            sb.Append(!string.IsNullOrEmpty(declineInfo.DeclineReason)
                ? declineInfo.DeclineReason
                : "Der blev ikke fundet nogen begrundelse");

            return sb.ToString();
        }

        private static string GetCommentAnswersAsString(List<string> answers, string hearingOwner)
        {
            if (answers == null || !answers.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var responder = !string.IsNullOrEmpty(hearingOwner) ? hearingOwner : "ukendt bruger";
            foreach (var answer in answers)
            {
                sb.Append($"Skrevet af {responder}: {answer}");
            }

            return sb.ToString();
        }
    }
}
