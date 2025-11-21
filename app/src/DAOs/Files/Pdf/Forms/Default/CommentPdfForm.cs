using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class CommentPdfForm : BasePdfForm
    {
        public CommentPdfForm(ITextResolver textResolver, IFileService fileService) : base(textResolver, fileService) { }

        public override Document GenerateContent()
        {
            var hearingData = HearingRecord.BaseData;

            Title = hearingData.Title;
            Subject = "Høringssvar";

            var section = AddSectionWithPageNumber();

            AddDocumentTop();

            AddBoldKeyNormalValuePair(section, "Sagsnummer", hearingData.EsdhNumber);
            AddBoldKeyNormalValuePair(section, "Høringstype", hearingData.HearingType);
            AddBoldKeyNormalValuePair(section, "Fagområde", hearingData.SubjectArea);
            AddBoldKeyNormalValuePair(section, "Svardato", $"{CommentRecord.Created:dd/MM/yyyy}");
            section.AddParagraph();
            AddBoldKeyNormalValuePair(section, "Svaret af", CommentRecord.ResponderName);

            section.AddParagraph("Beskrivelse", StyleNames.Heading4);

            section.AddParagraph(CommentRecord.CommentText);

            AddDocumentInfo();

            return Document;
        }
    }
}