using FluentValidation;

namespace Agora.Operations.Models.NotificationContentSpecifications.Queries
{
    public class GetNotificationContentSpecificationsQueryValidator : AbstractValidator<GetNotificationContentSpecificationsQuery>
    {
        public GetNotificationContentSpecificationsQueryValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
        }
    }
}