using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees
{
    public class UpdateInviteesBaseCommandValidator<TCommand> : AbstractValidator<TCommand> where TCommand : UpdateInviteesBaseCommand
    {
        protected UpdateInviteesBaseCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.InvitationSourceId).NotEqual(0);
        }
    }
}