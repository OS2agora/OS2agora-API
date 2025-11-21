using FluentValidation;

namespace Agora.Operations.Models.NotificationContentSpecifications.Commands
{
    public class CreateNotificationContentSpecificationCommandValidator : AbstractValidator<CreateNotificationContentSpecificationCommand>
    {
        public CreateNotificationContentSpecificationCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEmpty();
            RuleFor(c => c.NotificationTypeEnum).NotEmpty();
        }
    }
}