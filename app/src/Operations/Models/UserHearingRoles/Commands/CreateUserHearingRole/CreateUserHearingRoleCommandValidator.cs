using FluentValidation;

namespace BallerupKommune.Operations.Models.UserHearingRoles.Commands.CreateUserHearingRole
{
    public class CreateUserHearingRoleCommandValidator : AbstractValidator<CreateUserHearingRoleCommand>
    {
        public CreateUserHearingRoleCommandValidator()
        {
            RuleFor(c => c.UserHearingRole).NotNull()
                .ChildRules(userHearingRole =>
                {
                    userHearingRole.RuleFor(x => x.HearingRoleId).NotEqual(0);
                    userHearingRole.RuleFor(x => x.UserId).NotEqual(0);
                });
            RuleFor(c => c.HearingId).NotEqual(0);
        }
    }
}