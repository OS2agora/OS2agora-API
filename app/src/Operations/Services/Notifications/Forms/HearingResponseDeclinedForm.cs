using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class HearingResponseDeclinedForm : BaseNotificationForm
    {
        private readonly ICommentDao _commentDao;

        public HearingResponseDeclinedForm(ICommentDao commentDao, IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, 
            IFieldSystemResolver fieldSystemResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
            : base(globalContentDao, notificationContentSpecificationDao, notificationTypeDao, fieldSystemResolver, textResolver, options)
        {
            _commentDao = commentDao;
        }

        public override Task<NotificationContentResult> GetContentFromHearing(Hearing hearing)
        {
            throw new NotImplementedException();
        }

        public override async Task<NotificationContentResult> GetContentFromNotification(Notification notification)
        {
            var contents = GetNotificationContents(notification.NotificationType);

            var subject = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.SUBJECT);
            var header = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.HEADER);
            var body = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.BODY);
            var footer = GetContentByType(contents, Agora.Models.Enums.NotificationContentType.FOOTER);

            if (!notification.CommentId.HasValue)
            {
                throw new Exception($"Notification does not contain comment id for declined hearing response for hearing with id {notification!.HearingId}");
            }

            var declinedComment = await GetDeclinedCommentWithIncludes(notification.CommentId.Value);
            if (declinedComment == null)
            {
                throw new NotFoundException(nameof(Comment), notification.CommentId.Value);
            }

            var declinedCommentInfo = declinedComment.CommentDeclineInfo;
            if (declinedCommentInfo == null)
            {
                throw new Exception($"Declined comment with id {declinedComment.Id} does not have a CommentDeclineInfo - cannot create notification");
            }
            if (string.IsNullOrEmpty(declinedCommentInfo.DeclineReason))
            {
                throw new Exception($"CommentDeclineInfo for declined comment with id {declinedComment.Id} does not have a decline reason - cannot create notification");
            }

            var companyResponder = "";
            if (notification.Company != null)
            {
                companyResponder = $", afgivet på vegne af jer af {declinedComment.User.Name}, ";
            }

            var commentDeclineReason = $" \"{declinedCommentInfo.DeclineReason}\"";


            var notificationContent = BuildStandardForm(header, body, footer);
            var contentWithReplacedCustomVariables = ReplaceCustomVariablesAsync(notificationContent, declinedComment, commentDeclineReason, companyResponder);
            var contentWithReplacedCommonVariables = await ReplaceCommonVariablesAsync(contentWithReplacedCustomVariables, notification.Hearing);
            var subjectWithReplacedVariables = await ReplaceCommonVariablesAsync(subject, notification.Hearing);

            return new NotificationContentResult { Content = ReplaceNewLineVariable(contentWithReplacedCommonVariables), Subject = subjectWithReplacedVariables };
        }

        private static string ReplaceCustomVariablesAsync(string template, Comment declinedComment, string commentDeclineReason, string companyResponder)
        {
            var result = template;

            result = result.Replace("{{CommentNumber}}", $"\nSvar #{declinedComment.Number}");
            result = result.Replace("{{CommentDeclinedReason}}", commentDeclineReason);
            result = result.Replace("{{CompanyResponder}}", companyResponder);

            return result;
        }

        private async Task<Comment> GetDeclinedCommentWithIncludes(int commentId)
        {
            var commentIncludes = IncludeProperties.Create<Comment>(null, new List<string>
            {
                nameof(Comment.CommentDeclineInfo),
                nameof(Comment.User)
            });

            return await _commentDao.GetAsync(commentId, commentIncludes);
        }
    }
}