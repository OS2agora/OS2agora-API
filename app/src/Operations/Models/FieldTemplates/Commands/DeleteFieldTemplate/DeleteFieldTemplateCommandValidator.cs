using FluentValidation;

namespace Agora.Operations.Models.FieldTemplates.Commands.DeleteFieldTemplate
{
    public class DeleteFieldTemplateCommandValidator : AbstractValidator<DeleteFieldTemplateCommand>
    {
        public DeleteFieldTemplateCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
            RuleFor(c => c.HearingTypeId).NotEqual(0);
        }   
    }
}