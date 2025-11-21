using Agora.Models.Models.Invitations;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromPersonalInviteeIdentifiers
{
    public class UpdateInviteesFromPersonalInviteeIdentifiersCommandValidator : UpdateInviteesBaseCommandValidator<UpdateInviteesFromPersonalInviteeIdentifiersCommand>
    {
        public UpdateInviteesFromPersonalInviteeIdentifiersCommandValidator()
        {
            RuleFor(c => c.InviteeIdentifiers).NotEmpty();
            RuleForEach(c => c.InviteeIdentifiers).Must(HaveOnlyOneIdentifier);
        }

        private bool HaveOnlyOneIdentifier(InviteeIdentifiers identifier)
        {
            var identifiers = new List<string> { identifier.Cvr, identifier.Cpr, identifier.Email };
            var identifiersCount = identifiers.Count(s => !string.IsNullOrEmpty(s));
            return identifiersCount == 1;
        }
    }
}