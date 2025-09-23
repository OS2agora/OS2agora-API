using FluentValidation;

namespace BallerupKommune.Operations.Models.HearingTypes.Commands.UpdateHearingType
{
    public class UpdateHearingTypeCommandValidator : AbstractValidator<UpdateHearingTypeCommand>
    {
        public UpdateHearingTypeCommandValidator()
        {
            RuleFor(c => c.HearingType.Id).NotEqual(0);
            RuleFor(c => c.HearingType.Name).NotEmpty();
        }
    }
}