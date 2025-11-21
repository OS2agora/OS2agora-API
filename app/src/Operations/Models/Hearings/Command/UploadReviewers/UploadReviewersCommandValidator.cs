using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.UploadReviewers
{
    public class UploadReviewersCommandValidator : AbstractValidator<UploadReviewersCommand>
    {
        public UploadReviewersCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reviewers).NotNull();
            RuleFor(x => x.Reviewers).Must(reviewers =>
                reviewers.TrueForAll(reviewer =>
                    reviewer.HearingId != 0 &&
                    reviewer.UserId != 0 &&
                    reviewer.HearingRoleId != 0));
        }
    }
}
