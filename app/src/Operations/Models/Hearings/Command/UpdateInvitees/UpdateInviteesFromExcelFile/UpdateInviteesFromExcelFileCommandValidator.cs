using FluentValidation;
using System.Collections.Generic;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromExcelFile
{
    public class UpdateInviteesFromExcelFileCommandValidator : UpdateInviteesBaseCommandValidator<UpdateInviteesFromExcelFileCommand>
    {
        private static readonly List<string> AllowedFileExtensions = new List<string>
        {
            ".xlsx"
        };

        public UpdateInviteesFromExcelFileCommandValidator()
        {
            RuleFor(c => c.File.Extension)
                .Must(x => AllowedFileExtensions.Contains(x.ToLowerInvariant()))
                .WithMessage($"Only the following types of files are allowed: {string.Join(", ", AllowedFileExtensions)}");
        }
    }
}