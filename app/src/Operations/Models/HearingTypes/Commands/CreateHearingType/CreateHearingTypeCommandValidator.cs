using FluentValidation;

namespace Agora.Operations.Models.HearingTypes.Commands.CreateHearingType
{
    public class CreateHearingTypeCommandValidator : AbstractValidator<CreateHearingTypeCommand>
    {
        public CreateHearingTypeCommandValidator()
        {
            RuleFor(c => c.HearingType.HearingTemplateId).NotEmpty();
            RuleFor(c => c.HearingType.Name).NotEmpty();
            RuleFor(c => c.HearingType.IsActive).NotEqual(false);
        }
    }
}