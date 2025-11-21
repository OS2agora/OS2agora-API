using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.UpdateHearing.UpdateHearingFromCreatedStatus
{
    public class UpdateHearingFromCreatedStatusCommandValidator : AbstractValidator<UpdateHearingFromCreatedStatusCommand>
    {
        public UpdateHearingFromCreatedStatusCommandValidator()
        {
            RuleFor(h => h.Hearing.HearingStatusId).NotEmpty();
        }
    }
}
