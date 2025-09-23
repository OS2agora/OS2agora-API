using FluentValidation;

namespace BallerupKommune.Operations.Models.FieldTemplates.Commands.UpdateFieldTemplate
{
    public class UpdateFieldTemplateCommandValidator : AbstractValidator<UpdateFieldTemplateCommand>
    {
        public UpdateFieldTemplateCommandValidator()
        {
            RuleFor(c => c.FieldTemplate.FieldId).NotEmpty();
            RuleFor(c => c.FieldTemplate.HearingTypeId).NotEmpty();
            RuleFor(c => c.FieldTemplate.Name).NotEmpty();
            RuleFor(c => c.FieldTemplate.Text).NotEmpty();
        }
    }
}