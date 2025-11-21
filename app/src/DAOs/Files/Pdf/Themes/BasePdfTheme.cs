using Agora.DAOs.Files.Pdf.Constants;
using Agora.DAOs.Files.Pdf.Forms.Default;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;
using System.Collections.Generic;

namespace Agora.DAOs.Files.Pdf.Themes
{
    public class BasePdfTheme : IPdfTheme
    {
        private readonly ITextResolver _textResolver;
        private readonly IFileService _fileService;
        private readonly IHostingEnvironmentPath _envPath;

        public BasePdfTheme(ITextResolver textResolver, IFileService fileService, IHostingEnvironmentPath envPath)
        {
            _textResolver = textResolver;
            _fileService = fileService;
            _envPath = envPath;
        }

        public virtual IPdfForm GetHearingForm(HearingRecord hearingRecord)
            => new HearingPdfForm(_textResolver, _fileService) 
            {
                Document = InitializeDocument(),
                HearingRecord = hearingRecord
            };

        public virtual IPdfForm GetCommentForm(CommentRecord commentRecord, HearingRecord hearingRecord)
            => new CommentPdfForm(_textResolver, _fileService)
            {
                Document = InitializeDocument(),
                HearingRecord = hearingRecord,
                CommentRecord = commentRecord
            };

        public virtual IPdfForm GetConclusionForm(HearingRecord hearingRecord)
            => new ConclusionPdfForm(_textResolver, _fileService)
            {
                Document = InitializeDocument(),
                HearingRecord = hearingRecord,
            };

        public virtual IPdfForm GetFullHearingReportForm(HearingRecord hearingRecord)
            => new FullHearingReportPdfForm(_textResolver, _fileService, _envPath)
            {
                Document = InitializeDocument(),
                HearingRecord = hearingRecord
            };

        public virtual IPdfForm GetHearingReportForm(HearingRecord hearingRecord)
            => new HearingReportPdfForm(_textResolver, _fileService)
            {
                Document = InitializeDocument(),
                HearingRecord = hearingRecord
            };

        public virtual IPdfForm GetTextForm(List<string> content, string title, string subject)
            => new TextPdfForm(_textResolver, _fileService)
            {
                Document = InitializeDocument(),
                Title = title,
                Subject = subject,
                TextRecords = content
            };

        protected virtual Document InitializeDocument()
        {
            var document = new Document { Info = { Author = _textResolver.GetText(GroupKey.General, TextKey.MunicipalityName) } };
            DefineStyles(document);
            return document;

        }

        protected virtual void DefineStyles(Document document)
        {
            var normalStyle = document.Styles[StyleNames.Normal];
            normalStyle.ParagraphFormat.SpaceAfter = "12pt";
            normalStyle.Font.Size = 9;
            normalStyle.Font.Name = "Verdana";

            var heading1 = document.Styles[StyleNames.Heading1];
            heading1.Font.Size = 24;
            heading1.ParagraphFormat.SpaceAfter = "10pt";
            heading1.Font.Bold = true;

            var heading2 = document.Styles[StyleNames.Heading2];
            heading2.Font.Size = 14;
            heading2.ParagraphFormat.SpaceBefore = "12pt";
            heading2.ParagraphFormat.SpaceAfter = "8pt";

            var heading3 = document.Styles[StyleNames.Heading3];
            heading3.Font.Size = 12;
            heading3.ParagraphFormat.SpaceAfter = "8pt";

            var heading4 = document.Styles[StyleNames.Heading4];
            heading4.Font.Size = 11;
            heading4.ParagraphFormat.SpaceBefore = "12pt";

            var hyperlink = document.Styles[StyleNames.Hyperlink];
            hyperlink.Font.Underline = Underline.Single;
            hyperlink.Font.Color = Colors.Blue;

            var keyValueStyle = document.AddStyle(CustomStyles.KeyValuePair, StyleNames.Normal);
            keyValueStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);

            var orderedListStyle = document.AddStyle(CustomStyles.OrderedList, StyleNames.Normal);
            orderedListStyle.ParagraphFormat.LeftIndent = Unit.FromCentimeter(0.64);
            orderedListStyle.ParagraphFormat.SpaceAfter = "0pt";
            orderedListStyle.ParagraphFormat.SpaceBefore = "0pt";

            var unorderedListStyle = document.AddStyle(CustomStyles.UnorderedList, StyleNames.Normal);
            unorderedListStyle.ParagraphFormat.LeftIndent = Unit.FromCentimeter(0.64);
            unorderedListStyle.ParagraphFormat.SpaceAfter = "0pt";
            unorderedListStyle.ParagraphFormat.SpaceBefore = "0pt";

            var rowStyle = document.AddStyle(CustomStyles.Row, StyleNames.Normal);
            rowStyle.ParagraphFormat.SpaceBefore = Unit.FromPoint(2);
            rowStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

            var ReferenceStyle = document.Styles.AddStyle(CustomStyles.Reference, StyleNames.Normal);
            ReferenceStyle.ParagraphFormat.SpaceBefore = "5mm";
            ReferenceStyle.ParagraphFormat.SpaceAfter = "5mm";
            ReferenceStyle.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            var rightAlignedStyle = document.Styles.AddStyle(CustomStyles.RightAligned, StyleNames.Normal);
            rightAlignedStyle.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            var hrStyle = document.AddStyle(CustomStyles.HorizontalRule, StyleNames.Normal);
            var hrBorder = new Border
            {
                Width = "1pt",
                Color = Colors.DarkGray
            };
            hrStyle.ParagraphFormat.Borders.Bottom = hrBorder;
            hrStyle.ParagraphFormat.LineSpacing = 0;
            hrStyle.ParagraphFormat.SpaceBefore = 15;

            document.UseCmykColor = true;
        }
    }
}
