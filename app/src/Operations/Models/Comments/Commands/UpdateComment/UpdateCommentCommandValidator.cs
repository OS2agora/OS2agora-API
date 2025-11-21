using Agora.Models.Enums;
using Agora.Operations.Common.Validation;
using FluentValidation;
using System;
using System.Linq;

namespace Agora.Operations.Models.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandValidator : BaseAbstractValidator<UpdateCommentCommand>
    {
        const int MAX_FILE_COUNT = 10;
        const int MAX_FILE_NAME_LENGTH = 150;
        double MAX_FILE_SIZE = 10 * Math.Pow(10, 6); //10 MB

        public UpdateCommentCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.Text).NotEmpty();
            RuleFor(c => c.CommentStatus).NotNull();

            RuleFor(c => c.FileOperations.Where(fileOp => fileOp.Operation == FileOperationEnum.ADD))
                .Must(fileOpsAdd => fileOpsAdd.Count() <= MAX_FILE_COUNT)
                .WithMessage($"File count exceeds limit of {MAX_FILE_COUNT} files")
                .Must(fileOpsAdd => fileOpsAdd.All(f => f.File.Content.Length <= MAX_FILE_SIZE))
                .WithMessage($"At least one file exceeds file size limit of {MAX_FILE_SIZE} bytes")
                .Must(fileOpsAdd => fileOpsAdd.All(f => f.File.Name.Length <= MAX_FILE_NAME_LENGTH))
                .WithMessage($"At least one file exceeds the limit of characters in the file name of {MAX_FILE_NAME_LENGTH} characters");

            AddMunicipalitySpecificValidators();
        }

        protected override void RegisterCopenhagenValidators()
        {
            var allowedFileExtensions = new string[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".pdf"
            };

            // Validate MIME-types
            RuleFor(c => c.FileOperations.Where(fileOp => fileOp.Operation == FileOperationEnum.ADD))
                .Must(fileOpsAdd => fileOpsAdd.All(f =>
                    allowedFileExtensions.Contains(f.File.Extension, StringComparer.InvariantCultureIgnoreCase)))
                .WithMessage($"At least one file does not match the allowed file-types. Allowed types are: {string.Join(", ", allowedFileExtensions)}");
        }

        protected override void RegisterBallerupValidators()
        {
            // no custom validation rules
        }

        protected override void RegisterNovatarisValidators()
        {
            // no custom validation rules 
        }

        protected override void RegisterOS2Validators()
        {
            // no custom validation rules
        }
    }
}