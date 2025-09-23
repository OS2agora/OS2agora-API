using FluentValidation;

namespace BallerupKommune.Operations.Models.Fields.Commands.UpdateFields
{
    public class UpdateFieldsCommandValidator : AbstractValidator<UpdateFieldsCommand>
    {
        public UpdateFieldsCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEmpty();
            RuleFor(c => c.HearingStatusId).NotEmpty();
        }
    }
}