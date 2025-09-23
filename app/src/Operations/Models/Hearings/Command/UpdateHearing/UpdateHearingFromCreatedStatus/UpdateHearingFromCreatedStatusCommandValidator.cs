using FluentValidation;

namespace BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing.UpdateHearingFromCreatedStatus
{
    public class UpdateHearingFromCreatedStatusCommandValidator : AbstractValidator<UpdateHearingFromCreatedStatusCommand>
    {
        public UpdateHearingFromCreatedStatusCommandValidator()
        {
            RuleFor(h => h.Hearing.HearingStatusId).NotEmpty();
        }
    }
}
