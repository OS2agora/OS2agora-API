using BallerupKommune.Models.Models.Records;
using MigraDocCore.DocumentObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    public class HearingReportForm
    {
        public List<KeyValuePair<string, string>> BaseData = new List<KeyValuePair<string, string>>();
        public List<CommentRecord> CommentRecords = new List<CommentRecord>();
        public string Title = "";
        public string Subject = "";

        protected Document Document;

        private void DefineStyles()
        {
            var style = Document.Styles["Normal"];
            style.Font.Name = "Verdana";

            style = Document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = Document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // CreateForBodyInformation a new style called Title based on style Normal.
            style = Document.Styles.AddStyle("Title", "Normal");
            style.Font.Size = 9;

            // CreateForBodyInformation a new style called Reference based on style Normal.
            style = Document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create style for right aligned text
            style = Document.Styles.AddStyle("RightAligned", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            // Create style for making a horizontal rule
            var hr = Document.AddStyle("HorizontalRule", "Normal");
            var hrBorder = new Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Colors.DarkGray;
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;
        }

        protected void GenerateContent()
        {
            var section = Document.AddSection();
            section.PageSetup = Document.DefaultPageSetup.Clone();

            // Footer content
            // Right aligned page number on footer
            var paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Style = "RightAligned";
            paragraph.AddPageField();
            paragraph.AddText("/");
            paragraph.AddNumPagesField();
            // Centered
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("Ballerup Kommune");

            // Top of document
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = 0;
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Høringsjournal", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Ballerup Kommune, ");
            paragraph.AddDateField("dd.MM.yyyy");

            foreach (var (key, value) in BaseData)
            {
                paragraph = section.AddParagraph($"{key} - {value}");
            }

            foreach (var commentRecord in CommentRecords)
            {
                section.AddParagraph().Style = "HorizontalRule";
                section.AddParagraph($"#{commentRecord.Id}{(!string.IsNullOrEmpty(commentRecord.OnBehalfOf) ? " - På vegne af: " + commentRecord.OnBehalfOf : string.Empty)}").Format.Font.Bold = true;
                section.AddParagraph(string.Empty);section.AddParagraph(commentRecord.Comment);

                if (!string.IsNullOrEmpty(commentRecord.CommentDeclineInfo?.DeclineReason))
                {
                    section.AddParagraph("Høringsejers begrundelse for afvisning:").Format.Font.Bold = true;
                    section.AddParagraph("Skrevet af " + commentRecord.CommentDeclineInfo?.DeclinerInitials + ": " + commentRecord.CommentDeclineInfo.DeclineReason);
                }

                if (commentRecord.FileNames.Any())
                {
                    section.AddParagraph(string.Empty);
                    section.AddParagraph("Følgende filer er uploaded til høringssvaret:").Format.Font.Bold = true;

                    foreach (var fileName in commentRecord.FileNames)
                    {
                        section.AddParagraph(fileName);
                    }
                }

                if (commentRecord.AnswersToComment.Any())
                {
                    section.AddParagraph(string.Empty);
                    section.AddParagraph("Svar fra høringsejer:").Format.Font.Bold = true;

                    foreach (var answer in commentRecord.AnswersToComment)
                    {
                        section.AddParagraph("Skrevet af " + commentRecord.HearingOwnerDisplayName + ": " + answer);
                    }
                }
            }
        }

        public Document CreateDocument()
        {
            Document = new Document {Info = {Title = Title, Subject = Subject, Author = "Ballerup Kommune"}};

            DefineStyles();
            GenerateContent();
            return Document;
        }
    }
}