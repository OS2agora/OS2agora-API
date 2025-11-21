using FluentValidation;

namespace Agora.Operations.Models.HearingTypes.Commands.DeleteHearingType
{
    public class DeleteHearingTypeCommandValidator : AbstractValidator<DeleteHearingTypeCommand>
    {
        public DeleteHearingTypeCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}