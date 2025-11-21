using Agora.DAOs.Files.Pdf.Constants;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using DocumentFormat.OpenXml.Spreadsheet;
using MigraDoc.DocumentObjectModel;
using System;
using System.Linq;
using System.Text;

namespace Agora.DAOs.Files.Pdf.Forms.Default
{
    public class HearingReportPdfForm : BasePdfForm
    {
        public HearingReportPdfForm(ITextResolver textResolver, IFileService fileService) : base(textResolver, fileService) { }

        public override Document GenerateContent()
        {
            var hearingData = HearingRecord.BaseData;

            Title = $"{_textResolver.GetText(GroupKey.FileGeneration, TextKey.TitlePrefix)}{hearingData.Title}_{DateTime.Now}";
            Subject = "Rapport over høringssvar";

            var section = AddSectionWithPageNumber();

            AddDocumentTop();

            if (!string.IsNullOrEmpty(hearingData.EsdhNumber))
            {
                AddKeyValuePair(section, "Sagsnummer", hearingData.EsdhNumber, " - ");
            }

            AppendComments();
            AddDocumentInfo();

            return Document;
        }

        protected void AppendComments()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            foreach (var commentRecord in HearingRecord.CommentRecords)
            {
                if (!commentRecord.IsDeleted && commentRecord.Status != Agora.Models.Enums.CommentStatus.NOT_APPROVED)
                {
                    AppendComment(section, commentRecord);
                }
            }
        }

        protected void AppendComment(Section section, CommentRecord commentRecord)
        {
            section.AddParagraph(string.Empty, CustomStyles.HorizontalRule);
            section.AddParagraph($"#{commentRecord.Number}{(!string.IsNullOrEmpty(commentRecord.OnBehalfOf) ? " - På vegne af: " + commentRecord.OnBehalfOf : string.Empty)}").Format.Font.Bold = true;


            string commentText = BuildCommentText(commentRecord);

            section.AddParagraph(commentText);

            if (commentRecord.FileNames.Any())
            {
                section.AddParagraph("Følgende filer er uploaded til kommentaren:").Format.Font.Bold = true;

                foreach (var fileName in commentRecord.FileNames)
                {
                    section.AddParagraph(fileName);
                }
            }

            if (commentRecord.AnswersToComment.Any())
            {
                section.AddParagraph("Svar fra høringsejer:").Format.Font.Bold = true;

                foreach (var answer in commentRecord.AnswersToComment)
                {
                    section.AddParagraph("Skrevet af " + commentRecord.HearingOwnerDisplayName + ": " + answer);
                }
            }
        }

        protected string BuildCommentText(CommentRecord commentRecord)
        {
            StringBuilder commentText = new StringBuilder("");

            if (commentRecord?.Responder != null)
            {
                commentText.Append("Kommentar fra " + (commentRecord.Responder.EmployeeDisplayName ?? commentRecord.Responder.Name) + " " ?? "");
            }

            else if (commentRecord?.Company != null)
            {
                commentText.Append("Kommentar fra " + commentRecord.ResponderName + " (" + commentRecord.Company.Name + ")" + " " ?? "");
            }

            if (commentText.Length != 0 && !string.IsNullOrEmpty(commentRecord.OnBehalfOf))
            {
                commentText.Append("på vegne af " + commentRecord.OnBehalfOf + " ");
            }

            switch (commentRecord.Status)
            {
                case Agora.Models.Enums.CommentStatus.APPROVED:
                    commentText.Append(" (Godkendt høringssvar): ");
                    break;
                case Agora.Models.Enums.CommentStatus.AWAITING_APPROVAL:
                case Agora.Models.Enums.CommentStatus.NONE:
                    commentText.Append(" (Ikke godkendt høringssvar): ");
                    break;
            }

            commentText.Append(commentRecord.CommentText);

            return commentText.ToString();
        }
    }
}