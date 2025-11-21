using Agora.DAOs.Files.Pdf.Constants;
using Agora.DAOs.Files.Pdf.Extensions;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;
using System.Collections.Generic;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class TextPdfForm : BasePdfForm
    {
        public List<string> TextRecords { get; set; } = new List<string>();

        public TextPdfForm(ITextResolver textResolver, IFileService fileService) : base(textResolver, fileService)
        {
        }

        public override Document GenerateContent()
        {
            var section = AddSectionWithPageNumber();

            var paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = 0;
            paragraph.Style = CustomStyles.Reference;
            paragraph.AddTab();
            paragraph.AddText($"{_textResolver.GetText(GroupKey.General, TextKey.MunicipalityName)}, ");
            paragraph.AddDateField("dd.MM.yyyy");

            // Content of document
            foreach (var textRecord in TextRecords)
            {
                section.AddMarkdown(textRecord);
            }

            AddDocumentInfo();

            return Document;
        }
    }
}