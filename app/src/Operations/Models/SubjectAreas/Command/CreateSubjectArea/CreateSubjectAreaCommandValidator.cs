using FluentValidation;

namespace BallerupKommune.Operations.Models.SubjectAreas.Command.CreateSubjectArea
{
    public class CreateSubjectAreaCommandValidator : AbstractValidator<CreateSubjectAreaCommand>
    {
        public CreateSubjectAreaCommandValidator()
        {
            RuleFor(c => c.SubjectArea.Name).NotEmpty();
        }
    }
}
