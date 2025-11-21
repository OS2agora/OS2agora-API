using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.DeleteInvitationSourceMappings
{
    public class DeleteInvitationSourceMappingsCommandValidator : AbstractValidator<DeleteInvitationSourceMappingsCommand>
    {
        public DeleteInvitationSourceMappingsCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.InvitationSourceMappingIds).NotEmpty();
        }
    }
}