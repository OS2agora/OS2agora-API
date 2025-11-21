using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class ConclusionPdfForm : BasePdfForm
    {
        public ConclusionPdfForm(ITextResolver textResolver, IFileService fileService) : base(textResolver, fileService) { }

        public override Document GenerateContent()
        {
            Subject = "Høringskonklusion";

            var section = AddSectionWithPageNumber();

            AddDocumentTop();

            var hearingData = HearingRecord.BaseData;

            AddBoldKeyNormalValuePair(section, "Sagsnummer", hearingData.EsdhNumber);
            AddBoldKeyNormalValuePair(section, "Høringstype", hearingData.HearingType);
            AddBoldKeyNormalValuePair(section, "Fagområde", hearingData.SubjectArea);
            AddBoldKeyNormalValuePair(section, "Svardato", $"{hearingData.ConclusionCreatedDate:dd/MM/yyyy}");
            AddBoldKeyNormalValuePair(section, "Opsummering", hearingData.Summary);

            section.AddParagraph("Beskrivelse", StyleNames.Heading4);

            section.AddParagraph(hearingData.Conclusion);

            AddDocumentInfo();

            return Document;
        }
    }
}