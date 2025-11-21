using FluentValidation;

namespace Agora.Operations.Models.SubjectAreas.Command.UpdateSubjectArea
{
    public class UpdateSubjectAreaCommandValidator : AbstractValidator<UpdateSubjectAreaCommand>
    {
        public UpdateSubjectAreaCommandValidator()
        {
            RuleFor(c => c.SubjectArea.Id).NotEqual(0);
            RuleFor(c => c.SubjectArea.Name).Must(n => n != string.Empty);
        }
    }
}
