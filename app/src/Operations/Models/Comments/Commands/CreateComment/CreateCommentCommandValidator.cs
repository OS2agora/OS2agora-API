using BallerupKommune.Models.Enums;
using FluentValidation;

namespace BallerupKommune.Operations.Models.Comments.Commands.CreateComment
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.Text).NotEmpty();
            RuleFor(c => c.CommentType).NotEqual(CommentType.NONE);
        }
    }
}