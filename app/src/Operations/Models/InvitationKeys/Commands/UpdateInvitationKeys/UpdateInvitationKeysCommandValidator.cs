using Agora.Models.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Agora.Operations.Models.InvitationKeys.Commands.UpdateInvitationKeys
{
    public class UpdateInvitationKeysCommandValidator : AbstractValidator<UpdateInvitationKeysCommand>
    {
        public UpdateInvitationKeysCommandValidator()
        {
            RuleFor(c => c.InvitationGroupId).NotEmpty();
            RuleForEach(c => c.InvitationKeys).NotNull();
            RuleForEach(c => c.InvitationKeys).Must(HaveOnlyOneIdentifier)
                .ChildRules(invitationGroupMapping =>
                {
                    invitationGroupMapping.RuleFor(x => x.InvitationGroupId).NotEmpty();
                });
        }

        private bool HaveOnlyOneIdentifier(InvitationKey invitationKey)
        {
            var identifiers = new List<string> { invitationKey.Cvr, invitationKey.Cpr, invitationKey.Email };
            var identifiersCount = identifiers.Count(s => !string.IsNullOrEmpty(s));
            return identifiersCount == 1;
        }
    }
}