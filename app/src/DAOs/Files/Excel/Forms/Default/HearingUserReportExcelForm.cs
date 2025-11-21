using Agora.DAOs.Files.Excel.Extensions;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Agora.DAOs.Files.Excel.Forms.Default
{
    public class HearingUserReportExcelForm : BaseExcelForm
    {
        private static Dictionary<string, Type> Headers => new()
        {
            { "Navn", typeof(string) },
            { "Virksomhed", typeof(string) },
            { "Addresse", typeof(string) },
            { "By", typeof(string) },
            { "Postnummer", typeof(string) },
            { "CVR", typeof(string) },
            { "CPR", typeof(string) },
            { "Email", typeof(string) },
            { "Er inviteret", typeof(string) },
            { "Har besvaret", typeof(string) },
            { "Er virksomhed", typeof(string) }
        };

        public override XLWorkbook GenerateContent()
        {
            var hearingAnswerSheet = AddWorkSheet("Brugere");
            var table = GenerateTable(Headers);

            AddUserDataToTable(table);

            hearingAnswerSheet.AddTableToCell(table);

            return Workbook;
        }

        private void AddUserDataToTable(DataTable table)
        {
            foreach (var record in UserRecords)
            {
                var cpr = record.IsCompany ? null : record.Cpr;
                var cvr = record.IsCompany ? record.Company?.Cvr : null;
                var companyName = record.IsCompany ? record.Company?.Name : null;
                var address = record.IsCompany ? record.Company?.Address : record.Address;
                var city = record.IsCompany ? record.Company?.City : record.City;
                var postalCode = record.IsCompany ? record.Company?.PostalCode : record.PostalCode;

                table.Rows.Add(
                    record.Name,
                    companyName,
                    address,
                    city,
                    postalCode,
                    cvr,
                    cpr,
                    record.Email,
                    record.IsInvitee,
                    record.IsResponder,
                    record.IsCompany);
            }
        }
    }
}
