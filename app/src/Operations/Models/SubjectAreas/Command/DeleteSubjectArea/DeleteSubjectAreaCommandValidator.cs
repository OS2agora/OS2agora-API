using FluentValidation;

namespace BallerupKommune.Operations.Models.SubjectAreas.Command.DeleteSubjectArea
{
    public class DeleteSubjectAreaCommandValidator : AbstractValidator<DeleteSubjectAreaCommand>
    {
        public DeleteSubjectAreaCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}
