using System.Collections.Generic;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using System;

namespace BallerupKommune.DAOs.Files.Pdf.Forms
{
    public abstract class BaseForm
    {
        public List<KeyValuePair<string, string>> Header = new List<KeyValuePair<string, string>>();
        public string BodyText = "";
        public string Title = "";
        public string Subject = "";

        protected Document _document;
        private Table _table;

        private void DefineStyles()
        {
            var style = _document.Styles["Normal"];
            style.Font.Name = "Verdana";

            style = _document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = _document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // CreateForBodyInformation a new style called Title based on style Normal.
            style = _document.Styles.AddStyle("Title", "Normal");
            style.Font.Size = 9;

            // CreateForBodyInformation a new style called Reference based on style Normal.
            style = _document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create style for right aligned text
            style = _document.Styles.AddStyle("RightAligned", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            // Create style for making a horizontal rule
            var hr = _document.AddStyle("HorizontalRule", "Normal");
            var hrBorder = new Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Colors.DarkGray;
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;
        }

        protected void GenerateContent()
        {
            var section = _document.AddSection();
            section.PageSetup = _document.DefaultPageSetup.Clone();

            // CreateForBodyInformation the footer.
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

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = 0;
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Høringsjournal", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Ballerup Kommune, ");
            paragraph.AddDateField("dd.MM.yyyy");

            foreach (var kvp in Header)
            {
                paragraph = section.AddParagraph();
                if (kvp.Key.Contains("Svaret af", StringComparison.InvariantCultureIgnoreCase)) paragraph = section.AddParagraph();
                
                paragraph.AddText(kvp.Key);

                paragraph.AddText(kvp.Value);
                paragraph.Format.Font.Bold = false;
            }

            var p = section.AddParagraph();
            p.AddText("Beskrivelse");
            p.Format.Font.Bold = true;

            p = section.AddParagraph();
            p.AddText(BodyText);
        }

        public abstract Document CreateDocument();

        protected Document InternalCreateDocument()
        {
            _document = new Document();
            _document.Info.Title = Title;
            _document.Info.Subject = Subject;
            _document.Info.Author = "Ballerup Kommune";

            DefineStyles();
            GenerateContent();
            return _document;
        }
    }
}