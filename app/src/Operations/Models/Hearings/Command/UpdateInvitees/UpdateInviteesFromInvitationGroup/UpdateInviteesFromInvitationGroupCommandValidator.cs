using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromInvitationGroup
{
    public class UpdateInviteesFromInvitationGroupCommandValidator : UpdateInviteesBaseCommandValidator<UpdateInviteesFromInvitationGroupCommand>
    {
        public UpdateInviteesFromInvitationGroupCommandValidator()
        {
            RuleFor(c => c.InvitationGroupId).NotEqual(0);
        }
    }
}