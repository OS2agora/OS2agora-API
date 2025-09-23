using MigraDocCore.DocumentObjectModel;
using System.Collections.Generic;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    public class TextForm
    {
        public List<string> TextRecords = new List<string>();
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
            paragraph.AddText("Ballerup Kommune");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Top of document
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = 0;
            paragraph.Style = "Reference";
            paragraph.AddTab();
            paragraph.AddText("Ballerup Kommune, ");
            paragraph.AddDateField("dd.MM.yyyy");

            // Content of document
            foreach (var textRecord in TextRecords)
            { 
                section.AddParagraph(textRecord);
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