using FluentValidation;

namespace BallerupKommune.Operations.Models.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.Text).NotEmpty();
            RuleFor(c => c.CommentStatus).NotNull();
        }
    }
}