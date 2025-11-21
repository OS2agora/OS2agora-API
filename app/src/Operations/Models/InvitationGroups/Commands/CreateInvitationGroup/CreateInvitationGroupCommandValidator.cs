using FluentValidation;

namespace Agora.Operations.Models.InvitationGroups.Commands.CreateInvitationGroup
{
    public class CreateInvitationGroupCommandValidator : AbstractValidator<CreateInvitationGroupCommand>
    {
        public CreateInvitationGroupCommandValidator()
        {
            RuleFor(c => c.InvitationGroup.Name).NotEmpty();
            RuleFor(c => c.InvitationGroup.Name).MaximumLength(50);
        }
    }
}