using Agora.DAOs.Files.Pdf.Constants;
using Agora.DAOs.Files.Pdf.Extensions;
using Agora.Models.Enums;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.IO;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class FullHearingReportPdfForm : BasePdfForm
    {
        private readonly IHostingEnvironmentPath _envPath;

        public FullHearingReportPdfForm(ITextResolver textResolver, IFileService fileService, IHostingEnvironmentPath envPath) : base(textResolver, fileService)
        {
            _envPath = envPath;

        }

        public override Document GenerateContent()
        {
            var hearingData = HearingRecord.BaseData;
            Title = $"{_textResolver.GetText(GroupKey.FileGeneration, TextKey.TitlePrefix)}{hearingData.Title}_{DateTime.Now}";
            Subject = "Høringsrapport";

            AddFrontPage();
            AddReportContent();
            AddDocumentInfo();

            return Document;
        }

        private void AddFrontPage()
        {
            var section = AddFrontPageSection();
            
            var imageTable = section.AddTable();
            imageTable.Borders.Width = 0;

            var leftColumn = imageTable.AddColumn("13.5cm");
            var rightColumn = imageTable.AddColumn("4.5cm");

            var row = imageTable.AddRow();
            row.Height = "4cm";

            string basePath = Path.Combine(_envPath.WebRootPath, "images");
            var mainLogoPath = Path.Combine(basePath, _textResolver.GetText(GroupKey.PdfGeneration, TextKey.LogoPath));

            if (File.Exists(mainLogoPath))
            {
                var cell2 = row.Cells[1];
                var mainLogo = cell2.AddImage(mainLogoPath);
                mainLogo.Height = "3.12cm";
                mainLogo.Width = "3cm";
                cell2.Format.Alignment = ParagraphAlignment.Right;
                cell2.VerticalAlignment = VerticalAlignment.Top;
            }

            var titleParagraph = section.AddHeading1(HearingRecord.BaseData.Title);
            titleParagraph.Format.SpaceBefore = "1cm";
        }

        private void AddReportContent()
        {
            AddSectionWithPageNumber();
            AddHearingData();
            AddHearingSummary();
            AddHearingText();
            AddAttachmentInfo();

            AddSectionWithPageNumber();
            AddResponseSummary();
            AddResponseOverview();
            AddHearingResponses();

            AddHearingAttachments();

        }

        private void AddHearingData()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            var baseData = HearingRecord.BaseData;

            section.AddHeading2("Høringsdata:");

            AddBoldKeyNormalValuePair(section, "Høringstype", baseData.SubjectArea);
            AddBoldKeyNormalValuePair(section, "Bydel", baseData.CityArea);
            AddBoldKeyNormalValuePair(section, "Høringsperiode", $"{baseData.StartDate:dd. MMMM yyyy} til {baseData.Deadline:dd. MMMM yyyy}");
            //AddBoldKeyNormalValuePair(section, "Sagsnummer", baseData.EsdhNumber); Will be needed at a later point...
            AddBoldKeyNormalValuePair(section, "Høringsform", baseData.HearingType);
            AddBoldKeyNormalValuePair(section, "Svarform", baseData.ClosedHearing ? "Lukket" : "Åben");
        }

        private void AddHearingSummary()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            section.AddHeading2("Resume");
            section.AddParagraph(HearingRecord.BaseData.Summary);
        }

        private void AddHearingText()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            var hearingTextHeading = section.AddHeading2("Høringstekst");
           section.AddMarkdown(HearingRecord.BaseData.BodyText);
        }

        private void AddAttachmentInfo()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            var bilagParagraph = section.AddHeading3("Bilag vedhæftet høringsmaterialet vil være bagerst i PDF’en");
        }

        private void AddResponseSummary()
        {
            var commentStats = HearingRecord.CommentStats;

            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            section.AddHeading2("Høringssvar opsummeret");

            var pageWidth = GetPageWidth();
            var columnWidth = pageWidth / 2;

            var statsTable = section.AddTable();
            statsTable.Borders.Width = 0;
            statsTable.AddColumn(columnWidth);
            statsTable.AddColumn(columnWidth);

            var row = statsTable.AddRow();
            AddBoldKeyNormalValuePair(row, 0, "I alt", $"{commentStats.TotalResponses} svar");
            AddBoldKeyNormalValuePair(row, 1, "Svar afgivet på vegne af andre", $"{commentStats.OnBehalfOfResponses}");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
            

            row = statsTable.AddRow();
            AddBoldKeyNormalValuePair(row, 0, "Borger", $"{commentStats.CitizenResponses}");

            row = statsTable.AddRow();
            AddBoldKeyNormalValuePair(row, 0, "Virksomheder", $"{commentStats.CompanyResponses}");

            row = statsTable.AddRow();
            AddBoldKeyNormalValuePair(row, 0, "Medarbejdere", $"{commentStats.EmployeeResponses}");
        }

        private void AddResponseOverview()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation(true);
            section.AddHeading2("Oversigt over høringssvar");
            AddResponseOverviewTable();
        }

        private void AddHearingResponses()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            section.AddHeading3($"Høringssvar vedrørende: {HearingRecord.BaseData.Title}");

            foreach (var commentRecord in HearingRecord.CommentRecords)
            {
                if (commentRecord.Type != CommentType.HEARING_RESPONSE)
                {
                    continue;
                }
                section = GetSectionWithPageNumbersAndCorrectOrientation();
                AddResponse(section, commentRecord);
            }
        }

        private void AddResponse(Section section, CommentRecord commentRecord)
        {
            if (commentRecord.IsDeleted || commentRecord.Status != CommentStatus.APPROVED)
            {
                section.AddHeading4($"Svarnr.: {commentRecord.Number}");
                section.AddParagraph(commentRecord.IsDeleted ? "Slettet af bruger" : _textResolver.GetText(GroupKey.PdfGeneration, TextKey.DeclinedResponseText));
                return;
            }

            if (!string.IsNullOrEmpty(commentRecord.OnBehalfOf))
            {
                section.AddHeading4($"Svarnr.: {commentRecord.Number} På vegne af interesseorganisation eller borger {commentRecord.OnBehalfOf}");
            }
            else if (commentRecord.ResponderCapacity == UserCapacity.COMPANY)
            {
                section.AddHeading4($"Svarnr.: {commentRecord.Number} Virksomhed/Organisation");
                AddBoldKeyNormalValuePair(section, "Virksomhed/Organisation", $"{commentRecord.Company?.Name}");
                AddBoldKeyNormalValuePair(section, "Vejnavn", $"{commentRecord.Company?.StreetName}");
                AddBoldKeyNormalValuePair(section, "Postnr. og by", $"{commentRecord.Company?.PostalCode} {commentRecord.Company?.City}");
            }
            else if (commentRecord.ResponderCapacity == UserCapacity.CITIZEN)
            {
                section.AddHeading4($"Svarnr.: {commentRecord.Number} Borger");
                AddBoldKeyNormalValuePair(section, "Vejnavn", $"{commentRecord.Responder?.StreetName}");
                AddBoldKeyNormalValuePair(section, "Postnr. og by", $"{commentRecord.Responder?.PostalCode} {commentRecord.Responder?.City}");
            }
            else if (commentRecord.ResponderCapacity == UserCapacity.EMPLOYEE)
            {
                section.AddHeading4($"Svarnr.: {commentRecord.Number} Medarbejder i {_textResolver.GetText(GroupKey.General, TextKey.MunicipalityName)}");
            }

            AddBoldKeyNormalValuePair(section, "Høringssvar", commentRecord.CommentText, addLineBreak: true);

            if (commentRecord.Files.Count > 0)
            {
                AddAttachments(commentRecord.Files, commentRecord.Number);
            }
        }


        private void AddHearingAttachments()
        {
            if (HearingRecord.Attachments.Count > 0)
            {
                var section = AddSectionWithPageNumber();
                section.AddHeading2("Høringsbilag");
                AddAttachments(HearingRecord.Attachments);
            }
        }

        private void AddResponseOverviewTable()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation(true);
            var overview = section.AddTable();
            overview.Borders.Width = 0.75;

            AddResponseOverviewColumns(overview);
            AddResponseOverviewHeaderRow(overview);
            AddResponseOverviewData(overview);

        }

        private void AddResponseOverviewColumns(Table table)
        {
            // Response number
            table.AddColumn(Unit.FromCentimeter(1));
            // Responder
            table.AddColumn(Unit.FromCentimeter(4));
            // Company
            table.AddColumn(Unit.FromCentimeter(4));
            // On behalf of
            table.AddColumn(Unit.FromCentimeter(4));
            // Street name
            table.AddColumn(Unit.FromCentimeter(3.5));
            // Postal code
            table.AddColumn(Unit.FromCentimeter(2));
            // City
            table.AddColumn(Unit.FromCentimeter(3.5));
            // Changed
            table.AddColumn(Unit.FromCentimeter(2.5));
        }

        private void AddResponseOverviewHeaderRow(Table table)
        {
            var headerRow = table.AddRow();
            headerRow.Style = CustomStyles.Row;
            headerRow.Format.Font.Bold = true;

            headerRow.Cells[0].AddParagraph("ID");
            headerRow.Cells[1].AddParagraph("Indsendt af");
            headerRow.Cells[2].AddParagraph("Virksomhed / organisation");
            headerRow.Cells[3].AddParagraph("På vegne af");
            headerRow.Cells[4].AddParagraph("Vejnavn");
            headerRow.Cells[5].AddParagraph("Post nr.");
            headerRow.Cells[6].AddParagraph("By");
            headerRow.Cells[7].AddParagraph("Ændret");
        }

        private void AddResponseOverviewData(Table table)
        {
            foreach (var commentRecord in HearingRecord.CommentRecords)
            {
                if (commentRecord.Type != CommentType.HEARING_RESPONSE)
                {
                    continue;
                }

                AddResponseOverviewRow(table, commentRecord);
            }
        }

        private void AddResponseOverviewRow(Table table, CommentRecord commentRecord)
        {
            var row = table.AddRow();
            row.Style = CustomStyles.Row;
            row.Cells[0].AddParagraph($"{commentRecord.Number}");

            if (commentRecord.IsDeleted || commentRecord.Status != CommentStatus.APPROVED)
            {
                row.Cells[7].AddParagraph(commentRecord.IsDeleted ? "Slettet af bruger" : _textResolver.GetText(GroupKey.PdfGeneration, TextKey.DeclinedResponseText));
                return;
            }

            if (commentRecord.ResponderCapacity == UserCapacity.COMPANY)
            {
                row.Cells[1].AddParagraph("Virksomhed / organisation");
                row.Cells[2].AddParagraph(commentRecord.Company?.Name ?? string.Empty);
                row.Cells[4].AddParagraph(commentRecord.Company?.StreetName ?? string.Empty);
                row.Cells[5].AddParagraph(commentRecord.Company?.PostalCode ?? string.Empty);
                row.Cells[6].AddParagraph(commentRecord.Company?.City ?? string.Empty);
            }
            else if (commentRecord.ResponderCapacity == UserCapacity.CITIZEN)
            {
                row.Cells[1].AddParagraph("Borger");
                row.Cells[4].AddParagraph(commentRecord.Responder?.StreetName ?? string.Empty);
                row.Cells[5].AddParagraph(commentRecord.Responder?.PostalCode ?? string.Empty);
                row.Cells[6].AddParagraph(commentRecord.Responder?.City ?? string.Empty);
            }
            else if (commentRecord.ResponderCapacity == UserCapacity.EMPLOYEE)
            {
                row.Cells[1].AddParagraph("Medarbejder");
            }

            if (!string.IsNullOrEmpty(commentRecord.OnBehalfOf))
            {
                row.Cells[3].AddParagraph(commentRecord.OnBehalfOf);
            }
        }
    }
}
