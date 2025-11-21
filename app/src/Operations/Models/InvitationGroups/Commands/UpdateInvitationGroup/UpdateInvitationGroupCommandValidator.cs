using FluentValidation;

namespace Agora.Operations.Models.InvitationGroups.Commands.UpdateInvitationGroup
{
    public class UpdateInvitationGroupCommandValidator : AbstractValidator<UpdateInvitationGroupCommand>
    {
        public UpdateInvitationGroupCommandValidator()
        {
            RuleFor(c => c.InvitationGroup.Id).NotEqual(0);
            RuleFor(c => c.InvitationGroup.Name).NotEmpty();
            RuleFor(c => c.InvitationGroup.Name).MaximumLength(50);
        }
    }
}