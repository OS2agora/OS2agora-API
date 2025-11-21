using Agora.DAOs.Files.Pdf.Themes.Kobenhavn;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Fonts;
using System.IO;

namespace Agora.DAOs.Files.Pdf.Themes
{
    public class KobenhavnPdfTheme : BasePdfTheme
    {
        public KobenhavnPdfTheme(ITextResolver textResolver, IFileService fileService, IHostingEnvironmentPath envPath) : base(textResolver, fileService, envPath)
        {
            GlobalFontSettings.FontResolver = new KobenhavnFontResolver(Path.Combine(envPath.WebRootPath, "fonts"));
        }

        protected override void DefineStyles(Document document)
        {
            base.DefineStyles(document);
            var normalStyle = document.Styles[StyleNames.Normal];
            normalStyle.Font.Size = 10;
            normalStyle.Font.Name = "KBHTekst";

            var heading1 = document.Styles[StyleNames.Heading1];
            heading1.Font.Size = 22;
            heading1.ParagraphFormat.SpaceAfter = "10pt";
            heading1.Font.Bold = true;
            heading1.Font.Name = "KBH-Black";

            var heading2 = document.Styles[StyleNames.Heading2];
            heading2.Font.Size = 14;
            heading2.ParagraphFormat.SpaceBefore = "12pt";
            heading2.ParagraphFormat.SpaceAfter = "8pt";
            heading2.Font.Name = "KBH";

            var heading3 = document.Styles[StyleNames.Heading3];
            heading3.Font.Size = 12;
            heading3.ParagraphFormat.SpaceAfter = "8pt";
            heading3.Font.Name = "KBH";

            var heading4 = document.Styles[StyleNames.Heading4];
            heading4.Font.Size = 12;
            heading4.ParagraphFormat.SpaceBefore = "16pt";
            heading4.Font.Name = "KBH";
        }
    }
}
