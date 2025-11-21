using Agora.DAOs.Files.Excel.Extensions;
using Agora.Models.Enums;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Agora.DAOs.Files.Excel.Forms.Default
{
    public class HearingResponseReportExcelForm : BaseExcelForm
    {
        private static Dictionary<string, Type> Headers = new()
        {
            { "Svarnr.", typeof(int) },
            { "Navn", typeof(string) },
            { "Virksomhed", typeof(string) },
            { "Svar på vegne af", typeof(string) },
            { "Addresse", typeof(string) },
            { "By", typeof(string) },
            { "Postnummer", typeof(string) },
            { "CVR", typeof(string) },
            { "CPR", typeof(string) },
            { "Email", typeof(string) },
            { "Er inviteret", typeof(bool)},
            { "Er virksomhed", typeof(bool) },
            { "Høringssvar", typeof(string) },
            { "Svarstatus", typeof(string)},
            { "Afvist årsag", typeof(string) }

        };

        public override XLWorkbook GenerateContent()
        {
            var hearingAnswerSheet = AddWorkSheet("Brugere");
            var table = GenerateTable(Headers);

            AddCommentDataToTable(table);

            hearingAnswerSheet.AddTableToCell(table);

            return Workbook;
        }

        private void AddCommentDataToTable(DataTable table)
        {
            
            foreach (var record in CommentRecords.OrderBy(record => record.Number))
            {
                if (record.IsDeleted) continue;
                var userRecord = record.Responder;
                var companyRecord = userRecord?.Company;

                var isInvitee = userRecord.IsInvitee;
                var isCompany = userRecord.IsCompany;

                var responseNumber = record.Number;
                var name = record.ResponderName;
                var companyName = companyRecord?.Name;
                var onBehalfOf = record?.OnBehalfOf;
                var address = isCompany ? companyRecord?.Address : userRecord.Address;
                var city = isCompany ? companyRecord?.City : userRecord.City;
                var postalCode = isCompany ? companyRecord?.PostalCode : userRecord.PostalCode;
                var cvr = companyRecord?.Cvr;
                var cpr = userRecord.Cpr;
                var email = userRecord.Email;
                var response = record.CommentText;
                var status = CommentStatusAsString(record.Status);
                var declineReason = record?.CommentDeclineInfo?.DeclineReason;

                table.Rows.Add(responseNumber, name, companyName, onBehalfOf, address, city, postalCode, cvr, cpr,
                    email, isInvitee, isCompany, response, status, declineReason);

            }
        }

        private static string CommentStatusAsString(CommentStatus status)
        {
            switch (status)
            {
                case CommentStatus.APPROVED:
                    return "Accepteret";
                case CommentStatus.AWAITING_APPROVAL:
                    return "Afventer";
                case CommentStatus.NOT_APPROVED:
                    return "Afvist";
                default:
                    return string.Empty;

            }
        }
    }
}
