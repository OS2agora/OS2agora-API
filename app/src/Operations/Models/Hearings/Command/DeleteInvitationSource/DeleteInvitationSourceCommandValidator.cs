using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.DeleteInvitationSource
{
    public class DeleteInvitationSourceCommandValidator : AbstractValidator<DeleteInvitationSourceCommand>
    {
        public DeleteInvitationSourceCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.InvitationSourceName).NotEmpty();
        }
    }
}