using Agora.DAOs.Files.Pdf.Constants;
using Agora.Models.Models.Files;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Agora.DAOs.Files.Pdf.Forms
{
    public abstract class BasePdfForm : IPdfForm
    {
        public string Title = "";
        public string Subject = "";
        public string TempFolder = string.Empty;

        protected readonly ITextResolver _textResolver;
        protected readonly IFileService _fileService;

        public Document Document { get; set; }
        public HearingRecord HearingRecord { get; set; }
        public CommentRecord CommentRecord { get; set; }

        private static readonly string[] _allowedEmbededMimeTypes = { "application/pdf", "image/png", "image/jpeg" };
        private static readonly string[] _allowedEmbededFileTypes = { "pdf", "png", "jpeg" };

        protected BasePdfForm(ITextResolver textResolver, IFileService fileService)
        {
            _textResolver = textResolver;
            _fileService = fileService;
        }

        public abstract Document GenerateContent();

        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(TempFolder) && Directory.Exists(TempFolder))
            {
                Directory.Delete(TempFolder, true);
            }
        }

        public virtual void AddDocumentInfo()
        {
            Document.Info.Title = Title;
            Document.Info.Subject = Subject;
            Document.Info.Author = _textResolver.GetText(GroupKey.General, TextKey.MunicipalityName);
        }

        protected Unit GetPageWidth(bool isLandscape = false)
        {
            if (isLandscape)
            {
                return GetPageHeight();
            }
            return Document.DefaultPageSetup.PageWidth - Document.DefaultPageSetup.LeftMargin - Document.DefaultPageSetup.RightMargin;
        }

        protected Unit GetPageHeight(bool isLandscape = false)
        {
            if (isLandscape)
            {
                return GetPageWidth();
            }
            return Document.DefaultPageSetup.PageHeight - Document.DefaultPageSetup.TopMargin - Document.DefaultPageSetup.BottomMargin;
        }

        protected virtual void AddDocumentTop()
        {
            var section = GetSectionWithPageNumbersAndCorrectOrientation();
            var paragraph = section.AddParagraph("", CustomStyles.Reference);
            paragraph.Format.SpaceBefore = 0;
            paragraph.AddFormattedText(_textResolver.GetText(GroupKey.PdfGeneration, TextKey.DocumentHeader), TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText($"{_textResolver.GetText(GroupKey.General, TextKey.MunicipalityName)}, ");
            paragraph.AddDateField("dd.MM.yyyy");
        }

        protected virtual Section AddFrontPageSection()
        {
            return Document.AddSection();
        }

        protected virtual Section GetSectionWithPageNumbersAndCorrectOrientation(bool isLandscape = false)
        {
            var section = Document.LastSection;
            if (isLandscape && section.PageSetup.Orientation != Orientation.Landscape)
            {
                return AddSectionWithPageNumber(isLandscape);
            }

            if (!isLandscape && section.PageSetup.Orientation != Orientation.Portrait)
            {
                return AddSectionWithPageNumber();
            }

            return section;
        }

        protected virtual Section AddSectionWithPageNumber(bool isLandscape = false)
        {
            var section = Document.AddSection();
            section.PageSetup.Orientation = isLandscape ? Orientation.Landscape : Orientation.Portrait;

            AdjustPageMargins();
            AddFooter(isLandscape);

            return section;
        }

        protected virtual void AdjustPageMargins()
        {
            var section = Document.LastSection;
            section.PageSetup.BottomMargin = Unit.FromCentimeter(2.5);
        }

        protected virtual void AddFooter(bool isLandscape = false)
        {
            var section = Document.LastSection;
            var primaryFooter = section.Footers.Primary;

            var pageWidth = GetPageWidth(isLandscape);
            var columnWidth = pageWidth / 2;

            var footerTable = primaryFooter.AddTable();
            footerTable.Borders.Width = 0;
            footerTable.AddColumn(columnWidth);
            footerTable.AddColumn(columnWidth);

            var row = footerTable.AddRow();

            var municipalityStampParagraph = row.Cells[0].AddParagraph();
            municipalityStampParagraph.AddText(_textResolver.GetText(GroupKey.General, TextKey.MunicipalityName));
            municipalityStampParagraph.Format.Alignment = ParagraphAlignment.Left;
            municipalityStampParagraph.Format.Font.Size = 10;

            var pageNumberParagraph = row.Cells[1].AddParagraph();
            pageNumberParagraph.AddPageField();
            pageNumberParagraph.AddText(" af ");
            pageNumberParagraph.AddNumPagesField();
            pageNumberParagraph.Format.Alignment = ParagraphAlignment.Right;
            pageNumberParagraph.Format.Font.Size = 10;

        }

        protected void AddKeyValuePair(Section section, string key, string value, string separator = ": ")
        {
            section.AddParagraph($"{key}{separator}{value}", CustomStyles.KeyValuePair);
        }

        protected void AddBoldKeyNormalValuePair(Section section, string key, string value, string separator = ": ", bool addLineBreak = false)
        {
            var paragraph = section.AddParagraph("", CustomStyles.KeyValuePair);
            paragraph.AddFormattedText($"{key}{separator}", TextFormat.Bold);

            if (addLineBreak)
            {
                paragraph.AddLineBreak();
            }

            paragraph.AddText(value);
        }

        protected void AddBoldKeyNormalValuePair(TextFrame frame, string key, string value, string separator = ": ", int spaceAfter = 5)
        {
            var paragraph = frame.AddParagraph();
            paragraph.Format.SpaceAfter = Unit.FromPoint(spaceAfter);
            paragraph.AddFormattedText($"{key}{separator}", TextFormat.Bold);
            paragraph.AddText(value);
        }

        protected void AddBoldKeyNormalValuePair(Row row, int cell, string key, string value, string separator = ": ", int spaceAfter = 5)
        {
            row.Style = CustomStyles.Row;
            var paragraph = row.Cells[cell].AddParagraph();
            paragraph.AddFormattedText($"{key}{separator}", TextFormat.Bold);
            paragraph.AddText(value);
        }

        protected void AddAttachments(List<FileRecord> attachments, int? responseNumber = null)
        {
            var totalAttachments = attachments.Count();
            var currentAttachment = 1;

            var attachmentText = (int currentAttachment) =>
            {
                return responseNumber == null ? $"Høringsbilag {currentAttachment} af {totalAttachments}" : $"Bilag {currentAttachment} af {totalAttachments} for Svarnr. {responseNumber}";
            };

            foreach (var fileRecord in attachments)
            {
                var fileExtension = fileRecord.Extension.ToLower();
                if (!_allowedEmbededMimeTypes.Contains(fileExtension))
                {
                    var section = GetSectionWithPageNumbersAndCorrectOrientation();
                    AddBoldKeyNormalValuePair(section, attachmentText(currentAttachment), fileRecord.FileName);
                    section.AddParagraph($"Kan ikke renderer filen, da den ikke er i et af følgende formater: {_allowedEmbededFileTypes.Aggregate((acc, next) => $"{acc}, {next}")}");
                }
                else if (fileExtension == "application/pdf")
                {
                    AddPdfAttachment(fileRecord, attachmentText(currentAttachment));
                }
                else
                {
                    AddImageAttachment(fileRecord, attachmentText(currentAttachment));
                }

                currentAttachment++;
            }
        }

        protected void AddImageAttachment(FileRecord fileRecord, string attachmentText)
        {
            if (!System.IO.File.Exists(fileRecord.FilePath))
            {
                var section = GetSectionWithPageNumbersAndCorrectOrientation();
                AddBoldKeyNormalValuePair(section, attachmentText, fileRecord.FileName);
                section.AddParagraph($"Billedfilen '{Path.GetFileName(fileRecord.FileName)}' blev ikke fundet.");
                return;
            }

            using (var image = System.Drawing.Image.FromFile(fileRecord.FilePath))
            {
                var aspectRatio = (double)image.Width / image.Height;
                var isLandscape = aspectRatio > 1.3;

                var section = GetSectionWithPageNumbersAndCorrectOrientation(isLandscape);

                var pageWidth = GetPageWidth(isLandscape);
                var pageHeight = GetPageHeight(isLandscape) - Unit.FromCentimeter(1).Point;

                var imageWidth = Unit.FromPoint(image.Width);
                var imageHeight = Unit.FromPoint(image.Height);

                // Ensure that image dimensions fit pagedimensions
                if (imageWidth > pageWidth || imageHeight > pageHeight)
                {
                    var scaleWidth = pageWidth.Point / imageWidth.Point;
                    var scaleHeight = pageHeight.Point / imageHeight.Point;
                    var scaleFactor = Math.Min(scaleWidth, scaleHeight);

                    imageWidth = Unit.FromPoint(imageWidth.Point * scaleFactor);
                    imageHeight = Unit.FromPoint(imageHeight.Point * scaleFactor);
                }

                var frame = section.AddTextFrame();
                frame.Width = pageWidth;
                frame.Height = Unit.FromPoint(imageHeight.Point + Unit.FromCentimeter(1).Point);

                AddBoldKeyNormalValuePair(frame, attachmentText, fileRecord.FileName);

                var attachmentImage = frame.AddImage(fileRecord.FilePath);
                attachmentImage.Left = ShapePosition.Center;
                attachmentImage.Top = ShapePosition.Top;
                attachmentImage.LockAspectRatio = true;
                attachmentImage.Width = imageWidth;
                attachmentImage.Height = imageHeight;
            }
        }

        protected void AddPdfAttachment(FileRecord fileRecord, string attachmentText)
        {
            if (!System.IO.File.Exists(fileRecord.FilePath))
            {
                var section = GetSectionWithPageNumbersAndCorrectOrientation();
                AddBoldKeyNormalValuePair(section, attachmentText, fileRecord.FileName);
                section.AddParagraph($"Pdf-filen '{Path.GetFileName(fileRecord.FileName)}' blev ikke fundet.");
                return;
            }

            if (string.IsNullOrEmpty(TempFolder))
            {
                TempFolder = _fileService.GetDirectoryPath(HearingRecord.BaseData.Id, Path.GetRandomFileName());
            }

            var attachmentFolder = Path.Combine(TempFolder, Path.GetRandomFileName());

            var pagePaths = SplitPdfToPages(fileRecord.FilePath, attachmentFolder);
            var totalPages = pagePaths.Count();
            var currentPage = 1;

            foreach (var page in pagePaths)
            {
                var section = GetSectionWithPageNumbersAndCorrectOrientation(page.IsLandscape);
                var width = GetPageWidth(page.IsLandscape);

                var frame = section.AddTextFrame();
                frame.Width = width;
                frame.Height = GetPageHeight(page.IsLandscape);

                AddBoldKeyNormalValuePair(frame, attachmentText, $"{fileRecord.FileName} - Side {currentPage} af {totalPages}");
                var attachmentImage = frame.AddImage(page.Path);
                attachmentImage.Left = ShapePosition.Center;
                attachmentImage.Top = ShapePosition.Top;
                attachmentImage.LockAspectRatio = true;
                attachmentImage.Width = width;
                currentPage++;
            }
            
        }

        private List<PdfPageInfo> SplitPdfToPages(string pdfPath, string attatchmentFolder)
        {

            if (!Directory.Exists(attatchmentFolder))
            {
                Directory.CreateDirectory(attatchmentFolder);
            }

            List<PdfPageInfo> outputFilePaths = new();

            PdfDocument inputDocument = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);

            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                PdfDocument outputDocument = new PdfDocument();
                outputDocument.Version = inputDocument.Version;
                outputDocument.Info.Title = $"Page {i + 1}";
                outputDocument.Info.Creator = inputDocument.Info.Creator;

                var pdfPage = inputDocument.Pages[i];
                outputDocument.AddPage(pdfPage);

                string outputFilePath = Path.Combine(attatchmentFolder, $"Page_{i + 1}.pdf");

                outputDocument.Save(outputFilePath);

                outputFilePaths.Add(new PdfPageInfo
                {
                    Path = outputFilePath,
                    IsLandscape = pdfPage.Width > pdfPage.Height
                });
            }

            return outputFilePaths;
        }
    }
}