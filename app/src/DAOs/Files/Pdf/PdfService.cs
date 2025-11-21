using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using MigraDoc.Rendering;
using System.Collections.Generic;
using System.IO;

namespace Agora.DAOs.Files.Pdf
{
    public class PdfService : IPdfService
    {
        private readonly IPdfTheme _pdfTheme;
        public PdfService(IPdfTheme pdfTheme)
        {
            _pdfTheme = pdfTheme;
        }

        public byte[] CreateHearingPdf(HearingRecord hearingRecord)
        {
            var form = _pdfTheme.GetHearingForm(hearingRecord);

            return GeneratePdfFromForm(form);
        }

        public byte[] CreateConclusionPdf(HearingRecord hearingRecord)
        {
            var form = _pdfTheme.GetConclusionForm(hearingRecord); 

            return GeneratePdfFromForm(form);
        }

        public byte[] CreateCommentPdf(CommentRecord commentRecord, HearingRecord hearingRecord)
        {
            var form = _pdfTheme.GetCommentForm(commentRecord, hearingRecord);

            return GeneratePdfFromForm(form);
        }

        public byte[] CreateHearingReport(HearingRecord hearingRecord)
        {
            var form = _pdfTheme.GetHearingReportForm(hearingRecord); 

            return GeneratePdfFromForm(form);
        }

        public byte[] CreateFullHearingReport(HearingRecord hearingRecord)
        {
            var form = _pdfTheme.GetFullHearingReportForm(hearingRecord);

            return GeneratePdfFromForm(form);
        }

        public byte[] CreateTextPdf(List<string> content, string title, string subject)
        {
            var form = _pdfTheme.GetTextForm(content, title, subject);  

            return GeneratePdfFromForm(form);
        }

        private byte[] GeneratePdfFromForm(IPdfForm form)
        {
            byte[] result;
            try
            {
                var document = form.GenerateContent();

                var pdfRenderer = new PdfDocumentRenderer(true) { Document = document };

                pdfRenderer.RenderDocument();

                using var ms = new MemoryStream();
                pdfRenderer.Save(ms, false);
                result = ms.ToArray();

                form.CleanUp();
            }
            catch
            {
                form.CleanUp();
                throw;
            }
            

            return result;
        }
    }
}