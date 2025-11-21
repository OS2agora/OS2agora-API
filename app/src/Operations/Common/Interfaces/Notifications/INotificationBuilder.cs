namespace Agora.Operations.Common.Interfaces.Notifications
{
    public interface INotificationBuilder
    {
        INotificationForm GetAddedAsReviewerForm();
        INotificationForm GetInvitedToHearingForm();
        INotificationForm GetHearingAnswerReceiptForm();
        INotificationForm GetHearingConclusionPublishedForm();
        INotificationForm GetHearingChangedForm();
        INotificationForm GetHearingResponseDeclinedForm();
        INotificationForm GetDailyStatusForm();
        INotificationForm GetNewsLetterForm();
    }
}