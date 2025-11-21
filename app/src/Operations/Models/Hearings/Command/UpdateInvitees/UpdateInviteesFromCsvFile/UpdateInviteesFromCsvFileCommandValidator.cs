using FluentValidation;
using System.Collections.Generic;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromCsvFile
{
    public class UpdateInviteesFromCsvFileCommandValidator : UpdateInviteesBaseCommandValidator<UpdateInviteesFromCsvFileCommand>
    {
        private static readonly List<string> AllowedFileExtensions = new List<string>
        {
            ".csv"
        };

        public UpdateInviteesFromCsvFileCommandValidator()
        {
            RuleFor(c => c.File.Extension)
                .Must(x => AllowedFileExtensions.Contains(x.ToLowerInvariant()))
                .WithMessage($"Only the following types of files are allowed: {string.Join(", ", AllowedFileExtensions)}");
        }
    }
}