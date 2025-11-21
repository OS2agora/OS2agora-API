using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Notifications;
using Agora.Operations.Resolvers;
using Agora.Operations.Services.Notifications.Forms;
using Microsoft.Extensions.Options;

namespace Agora.Operations.Services.Notifications
{
    public class BaseNotificationBuilder : INotificationBuilder
    {
        private readonly ICommentDao _commentDao;
        private readonly IEventMappingDao _eventMappingDao;
        private readonly IGlobalContentDao _globalContentDao;
        private readonly INotificationTypeDao _notificationTypeDao;
        private readonly INotificationContentSpecificationDao _notificationContentSpecificationDao;
        private readonly IUserDao _userDao;

        private readonly IFieldSystemResolver _fieldSystemResolver;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private readonly ITextResolver _textResolver;

        private readonly IOptions<AppOptions> _options;

        public BaseNotificationBuilder(ICommentDao commentDao, IEventMappingDao eventMappingDao, IGlobalContentDao globalContentDao, 
            INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, IUserDao userDao,
            IFieldSystemResolver fieldSystemResolver, IHearingRoleResolver hearingRoleResolver, ITextResolver textResolver,
            IOptions<AppOptions> options)
        {
            _commentDao = commentDao;
            _eventMappingDao = eventMappingDao;
            _globalContentDao = globalContentDao;
            _notificationContentSpecificationDao = notificationContentSpecificationDao;
            _notificationTypeDao = notificationTypeDao;
            _userDao = userDao;

            _fieldSystemResolver = fieldSystemResolver;
            _hearingRoleResolver = hearingRoleResolver;
            _textResolver = textResolver;

            _options = options;
        }

        public virtual INotificationForm GetAddedAsReviewerForm()
        {
            return new AddedAsReviewerForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetInvitedToHearingForm()
        {
            return new InvitedToHearingForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetHearingAnswerReceiptForm()
        {
            return new HearingAnswerReceiptForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _userDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetHearingConclusionPublishedForm()
        {
            return new HearingConclusionPublishedForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetHearingChangedForm()
        {
            return new HearingChangedForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetHearingResponseDeclinedForm()
        {
            return new HearingResponseDeclinedForm(_commentDao, _globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetDailyStatusForm()
        {
            return new DailyStatusForm(_eventMappingDao, _globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _hearingRoleResolver, _textResolver, _options);
        }

        public virtual INotificationForm GetNewsLetterForm()
        {
            return new NewsLetterForm(_globalContentDao, _notificationContentSpecificationDao, _notificationTypeDao, _fieldSystemResolver, _textResolver, _options);
        }
    }
}