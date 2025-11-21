using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class HearingPdfForm : BasePdfForm
    {
        public HearingPdfForm(ITextResolver textResolver, IFileService fileService) : base(textResolver, fileService) { }

        public override Document GenerateContent()
        {
            var hearingData = HearingRecord.BaseData;

            Title = hearingData.Title;
            Subject = "Høringsjournal";

            var section = AddSectionWithPageNumber();

            AddDocumentTop();

            AddBoldKeyNormalValuePair(section, "Sagsnummer", hearingData.EsdhNumber);
            AddBoldKeyNormalValuePair(section, "Høringstype", hearingData.HearingType);
            AddBoldKeyNormalValuePair(section, "Fagområde", hearingData.SubjectArea);
            AddBoldKeyNormalValuePair(section, "Startdato", $"{hearingData.StartDate:dd/MM/yyyy}");
            AddBoldKeyNormalValuePair(section, "Slutdato", $"{hearingData.Deadline:dd/MM/yyyy}");
            AddBoldKeyNormalValuePair(section, "Opsummering", hearingData.Summary);

            section.AddParagraph("Beskrivelse", StyleNames.Heading4);

            section.AddParagraph(hearingData.BodyText);

            AddDocumentInfo();

            return Document;
        }
    }
}