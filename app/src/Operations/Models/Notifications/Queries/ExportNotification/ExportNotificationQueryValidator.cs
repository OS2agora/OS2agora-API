using FluentValidation;

namespace Agora.Operations.Models.Notifications.Queries.ExportNotification
{
    public class ExportNotificationQueryValidator : AbstractValidator<ExportNotificationQuery>
    {
        public ExportNotificationQueryValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
            RuleFor(c => c.NotificationType).NotEmpty();
        }   
    }
}