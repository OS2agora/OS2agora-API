using FluentValidation;

namespace BallerupKommune.Operations.Models.Users.Command.LoginUser
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(loginUserCommand => loginUserCommand.TokenUser).NotNull();
            RuleFor(loginUserCommand => loginUserCommand.TokenUser.ApplicationUserId).NotEmpty();
            RuleFor(loginUserCommand => loginUserCommand.TokenUser.AuthMethod).IsInEnum();
            RuleFor(loginUserCommand => loginUserCommand.TokenUser.Name).NotEmpty();
            RuleFor(loginUserCommand => loginUserCommand.TokenUser.PersonalIdentifier).NotEmpty();
        }
    }
}