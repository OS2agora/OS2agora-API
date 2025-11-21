using FluentValidation;

namespace Agora.Operations.Models.SubjectAreas.Command.CreateSubjectArea
{
    public class CreateSubjectAreaCommandValidator : AbstractValidator<CreateSubjectAreaCommand>
    {
        public CreateSubjectAreaCommandValidator()
        {
            RuleFor(c => c.SubjectArea.Name).NotEmpty();
        }
    }
}
