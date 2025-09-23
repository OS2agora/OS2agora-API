using BallerupKommune.DAOs.Files.Pdf.Forms;
using BallerupKommune.Models.Models.Records;
using BallerupKommune.Operations.Common.Interfaces;
using MigraDocCore.Rendering;
using System;
using System.Collections.Generic;
using System.IO;

namespace BallerupKommune.DAOs.Files.Pdf
{
    public class PdfService : IPdfService
    {
        public byte[] CreateHearingPdf(string hearingTitle, string hearingSummary, string hearingBodyInformation,
            string esdhNumber, string hearingType, string subjectArea, DateTime startDate, DateTime deadline)
        {
            var form = new HearingForm
            {
                Title = hearingTitle,
                Summary = hearingSummary,
                Description = hearingBodyInformation,
                CaseNumber = esdhNumber,
                HearingType = hearingType,
                SubjectArea = subjectArea,
                StartDate = startDate,
                EndDate = deadline
            };
            return GeneratePdfFromForm(form);
        }

        public byte[] CreateConclusionPdf(string esdhNumber, string hearingSummary, string hearingType,
            string subjectArea, string conclusionText, DateTime conclusionDate)
        {
            var form = new ConclusionForm
            {
                CaseNumber = esdhNumber,
                ConclusionBody = conclusionText,
                ConclusionDate = conclusionDate,
                Summary = hearingSummary,
                SubjectArea = subjectArea,
                HearingType = hearingType
            };
            return GeneratePdfFromForm(form);
        }

        public byte[] CreateCommentPdf(string hearingTitle, string commentWriterName, string commentText, string esdhNumber, string hearingType,
            string subjectArea, DateTime created)
        {
            var form = new CommentForm
            {
                ResponseWriterName = commentWriterName,
                ResponseBody = commentText,
                CaseNumber = esdhNumber,
                HearingType = hearingType,
                ResponseDate = created,
                SubjectArea = subjectArea,
                Title = hearingTitle
            };
            return GeneratePdfFromForm(form);
        }

        public byte[] CreateHearingReport(string hearingTitle, string esdhNumber, List<CommentRecord> commentRecords)
        {
            var form = new HearingReportForm
            {
                Title = hearingTitle,
                Subject = "Rapport over høringssvar",
                CommentRecords = commentRecords,
                BaseData = !string.IsNullOrEmpty(esdhNumber) ? new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Sagsnummer", esdhNumber),
                } : new List<KeyValuePair<string, string>>()
            };
            return GeneratePdfFromForm(form);
        }

        public byte[] CreateTextPdf(List<string> content, string title, string subject)
        {
            var form = new TextForm
            {
                Title = title,
                Subject = subject,
                TextRecords = content
            };
            return GeneratePdfFromForm(form);
        }

        private byte[] GeneratePdfFromForm(BaseForm form)
        {
            var document = form.CreateDocument();
            document.UseCmykColor = true;

            var pdfRenderer = new PdfDocumentRenderer(true) { Document = document };

            pdfRenderer.RenderDocument();

            using var ms = new MemoryStream();
            pdfRenderer.Save(ms, false);
            var result = ms.ToArray();

            return result;
        }

        private byte[] GeneratePdfFromForm(HearingReportForm form)
        {
            var document = form.CreateDocument();
            document.UseCmykColor = true;

            var pdfRenderer = new PdfDocumentRenderer(true) { Document = document };

            pdfRenderer.RenderDocument();

            using var ms = new MemoryStream();
            pdfRenderer.Save(ms, false);
            var result = ms.ToArray();

            return result;
        }

        private byte[] GeneratePdfFromForm(TextForm form)
        {
            var document = form.CreateDocument();
            document.UseCmykColor = true;

            var pdfRenderer = new PdfDocumentRenderer(true) { Document = document };

            pdfRenderer.RenderDocument();

            using var ms = new MemoryStream();
            pdfRenderer.Save(ms, false);
            var result = ms.ToArray();

            return result;
        }
    }
}