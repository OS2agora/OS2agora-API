using FluentValidation;

namespace Agora.Operations.Models.Notifications.Queries.GetInvitationNotifications
{
    public class GetInvitationNotificationsQueryValidator : AbstractValidator<GetInvitationNotificationsQuery>
    {
        public GetInvitationNotificationsQueryValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
        }
    }
}