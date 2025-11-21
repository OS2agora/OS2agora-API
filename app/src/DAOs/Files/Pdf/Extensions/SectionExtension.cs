using Agora.DAOs.Files.Pdf.Utils;
using MigraDoc.DocumentObjectModel;

namespace Agora.DAOs.Files.Pdf.Extensions
{
    public static class SectionExtension
    {
        public static Paragraph AddHeading1(this Section section, string text)
        {
            return section.AddStyledParagraph(text, StyleNames.Heading1);
        }

        public static Paragraph AddHeading2(this Section section, string text)
        {
            return section.AddStyledParagraph(text, StyleNames.Heading2);
        }

        public static Paragraph AddHeading3(this Section section, string text)
        {
            return section.AddStyledParagraph(text, StyleNames.Heading3);
        }

        public static Paragraph AddHeading4(this Section section, string text)
        {
            return section.AddStyledParagraph(text, StyleNames.Heading4);
        }

        public static void AddMarkdown(this Section section, string markdown)
        {
            string html = MarkdownParser.ConvertMarkdownToHtml(markdown);
            section.AddHtml(html);
        }

        public static void AddHtml(this Section section, string html)
        {
            SectionHtmlParser.ParseHtmlToSection(section, html);
        }

        private static Paragraph AddStyledParagraph(this Section section, string text, string style) 
        {
            return section.AddParagraph(text, style);
        }
    }
}
