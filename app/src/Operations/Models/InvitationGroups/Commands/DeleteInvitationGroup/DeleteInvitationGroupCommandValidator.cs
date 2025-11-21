using FluentValidation;

namespace Agora.Operations.Models.InvitationGroups.Commands.DeleteInvitationGroup
{
    public class DeleteInvitationGroupCommandValidator : AbstractValidator<DeleteInvitationGroupCommand>
    {
        public DeleteInvitationGroupCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}