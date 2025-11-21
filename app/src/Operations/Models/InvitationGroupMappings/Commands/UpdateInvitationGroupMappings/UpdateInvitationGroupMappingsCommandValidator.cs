using FluentValidation;

namespace Agora.Operations.Models.InvitationGroupMappings.Commands.UpdateInvitationGroupMappings
{
    public class UpdateInvitationGroupMappingsCommandValidator : AbstractValidator<UpdateInvitationGroupMappingsCommand>
    {
        public UpdateInvitationGroupMappingsCommandValidator()
        {
            RuleFor(c => c.HearingTypeId).NotEmpty();
            RuleForEach(c => c.InvitationGroupMappings).NotNull()
                .ChildRules(invitationGroupMapping =>
                {
                    invitationGroupMapping.RuleFor(x => x.InvitationGroupId).NotEmpty();
                    invitationGroupMapping.RuleFor(x => x.HearingTypeId).NotEmpty();
                });
        }
    }
}