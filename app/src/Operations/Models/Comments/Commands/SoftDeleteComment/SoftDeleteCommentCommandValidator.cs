using FluentValidation;

namespace Agora.Operations.Models.Comments.Commands.SoftDeleteComment
{
    public class SoftDeleteCommentCommandValidator : AbstractValidator<SoftDeleteCommentCommand>
    {
        public SoftDeleteCommentCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}