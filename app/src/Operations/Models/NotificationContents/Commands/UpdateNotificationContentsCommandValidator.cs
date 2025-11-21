using FluentValidation;

namespace Agora.Operations.Models.NotificationContents.Commands
{
    public class UpdateNotificationContentsCommandValidator : AbstractValidator<UpdateNotificationContentsCommand>
    {
        public UpdateNotificationContentsCommandValidator()
        {
            RuleFor(c => c.HearingId).NotEmpty();
            RuleFor(c => c.NotificationContentSpecificationId).NotEmpty();
            RuleFor(c => c.NotificationContents).NotEmpty();
            RuleForEach(c => c.NotificationContents).NotEmpty()
                .ChildRules(notificationContent =>
                {
                    notificationContent.RuleFor(x => x.Id).NotEmpty();
                });
        }
    }
}