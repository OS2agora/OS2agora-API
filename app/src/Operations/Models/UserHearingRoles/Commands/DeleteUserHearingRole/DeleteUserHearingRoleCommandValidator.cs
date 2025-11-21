using FluentValidation;

namespace Agora.Operations.Models.UserHearingRoles.Commands.DeleteUserHearingRole
{
    public class DeleteUserHearingRoleCommandValidator : AbstractValidator<DeleteUserHearingRoleCommand>
    {
        public DeleteUserHearingRoleCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}