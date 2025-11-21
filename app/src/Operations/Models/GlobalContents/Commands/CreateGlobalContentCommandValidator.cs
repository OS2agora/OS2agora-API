using FluentValidation;

namespace Agora.Operations.Models.GlobalContents.Commands
{
    public class CreateGlobalContentCommandValidator : AbstractValidator<CreateGlobalContentCommand>
    {
        public CreateGlobalContentCommandValidator()
        {
            RuleFor(g => g.GlobalContent).NotNull();
            RuleFor(c => c.GlobalContent.Content).NotEmpty();
            RuleFor(t => t.GlobalContentTypeId).NotEmpty();
        }
    }
}