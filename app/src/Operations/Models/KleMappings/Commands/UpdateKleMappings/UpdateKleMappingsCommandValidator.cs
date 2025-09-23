using FluentValidation;

namespace BallerupKommune.Operations.Models.KleMappings.Commands.UpdateKleMappings
{
    public class UpdateKleMappingsCommandValidator : AbstractValidator<UpdateKleMappingsCommand>
    {
        public UpdateKleMappingsCommandValidator()
        {
            RuleFor(c => c.HearingTypeId).NotEmpty();
            RuleForEach(c => c.KleMappings).NotNull()
                .ChildRules(kleMapping =>
                {
                    kleMapping.RuleFor(x => x.KleHierarchyId).NotEmpty();
                    kleMapping.RuleFor(x => x.HearingTypeId).NotEmpty();
                });
        }
        
    }
}