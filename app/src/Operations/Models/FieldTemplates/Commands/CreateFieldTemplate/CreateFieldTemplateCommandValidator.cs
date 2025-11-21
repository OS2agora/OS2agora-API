using Agora.Operations.Models.FieldTemplates.Commands.Rules;
using FluentValidation;
namespace Agora.Operations.Models.FieldTemplates.Commands.CreateFieldTemplate
{
    public class CreateFieldTemplateCommandValidator : AbstractValidator<CreateFieldTemplateCommand>
    {
        public CreateFieldTemplateCommandValidator()
        {
            RuleFor(c => c.FieldTemplate.HearingTypeId).NotEmpty();
            RuleFor(c => c.FieldTemplate.FieldId).NotEmpty();
            RuleFor(c => c.FieldTemplate.Name).NotEmpty();
            RuleFor(c => c.FieldTemplate.Text).NotEmpty();
            RuleFor(c => c.FieldTemplate.Text.Length).LessThanOrEqualTo(FieldTemplateConstants.TextFieldMaximumLength);
        }
    }
} 