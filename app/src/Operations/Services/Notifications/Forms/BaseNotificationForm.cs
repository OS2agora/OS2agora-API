using Agora.Models.Common;
using Agora.Models.Extensions;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Notifications;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FieldType = Agora.Models.Enums.FieldType;
using GlobalContentType = Agora.Models.Enums.GlobalContentType;
using NotificationContentType = Agora.Models.Enums.NotificationContentType;
using NotificationTypeEnum = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Services.Notifications.Forms
{
    public abstract class BaseNotificationForm : INotificationForm
    {
        private readonly IGlobalContentDao _globalContentDao;
        private readonly INotificationContentSpecificationDao _notificationContentSpecificationDao;
        private readonly INotificationTypeDao _notificationTypeDao;

        private readonly IFieldSystemResolver _fieldSystemResolver;
        private readonly ITextResolver _textResolver;

        private readonly IOptions<AppOptions> _options;

        protected BaseNotificationForm(IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, 
            IFieldSystemResolver fieldSystemResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
        {
            _globalContentDao = globalContentDao;
            _notificationContentSpecificationDao = notificationContentSpecificationDao;
            _notificationTypeDao = notificationTypeDao;

            _fieldSystemResolver = fieldSystemResolver;
            _textResolver = textResolver;

            _options = options;
        }

        public abstract Task<NotificationContentResult> GetContentFromHearing(Hearing hearing);

        public abstract Task<NotificationContentResult> GetContentFromNotification(Notification notification);

        protected async Task<NotificationContentResult> GetStandardNotificationContent(Hearing hearing, NotificationTypeEnum notificationTypeEnum)
        {
            var notificationTypeIncludes = IncludeProperties.Create<NotificationType>(null, new List<string>
            {
                nameof(NotificationType.SubjectTemplate),
                nameof(NotificationType.HeaderTemplate),
                nameof(NotificationType.BodyTemplate),
                nameof(NotificationType.FooterTemplate)
            });
            
            var allNotificationTypes = await _notificationTypeDao.GetAllAsync(notificationTypeIncludes);
            var notificationType = allNotificationTypes.FirstOrDefault(nt => nt.Type == notificationTypeEnum);
            var notificationContentSpecification = hearing.NotificationContentSpecifications.SingleOrDefault(ncs => ncs.NotificationType.Type == notificationTypeEnum);

            var contents = GetNotificationContents(notificationType, notificationContentSpecification);

            return await GetStandardNotificationContentResult(hearing, contents);
        }

        protected async Task<NotificationContentResult> GetStandardNotificationContent(Notification notification)
        {
            List<ContentAndType> contents;
            if (notification.HearingId.HasValue)
            {
                var notificationContentSpecification =
                    await GetNotificationContentSpecificationForHearingAndNotificationType(notification.HearingId.Value,
                        notification.NotificationTypeId);
                contents = notificationContentSpecification == null 
                    ? GetNotificationContents(notification.NotificationType) 
                    : GetNotificationContents(notification.NotificationType, notificationContentSpecification);
            }
            else
            {
                contents = GetNotificationContents(notification.NotificationType);
            }

            return await GetStandardNotificationContentResult(notification.Hearing, contents);
        }

        protected async Task<NotificationContentResult> GetStandardNotificationContentFromTemplate(Notification notification)
        {
            var contents = GetNotificationContents(notification.NotificationType);
            return await GetStandardNotificationContentResult(notification.Hearing, contents);
        }

        private async Task<NotificationContentResult> GetStandardNotificationContentResult(Hearing hearing, List<ContentAndType> contents)
        {
            var subject = GetContentByType(contents, NotificationContentType.SUBJECT);
            var header = GetContentByType(contents, NotificationContentType.HEADER);
            var body = GetContentByType(contents, NotificationContentType.BODY);
            var footer = GetContentByType(contents, NotificationContentType.FOOTER);

            var notificationContent = BuildStandardForm(header, body, footer);
            var contentWithReplacedVariables = await ReplaceCommonVariablesAsync(notificationContent, hearing);
            var subjectWithReplacedVariables = await ReplaceCommonVariablesAsync(subject, hearing);
            return new NotificationContentResult { Content = ReplaceNewLineVariable(contentWithReplacedVariables), Subject = subjectWithReplacedVariables };
        }

        protected string BuildStandardForm(string header, string body, string footer)
        {
            return header + "{{NewLine}}" + body + "{{NewLine}}" + footer;
        }

        protected async Task<string> ReplaceCommonVariablesAsync(string template, Hearing hearing)
        {
            var result = template;

            if (result.Contains("{{HearingTitle}}"))
            {
                var hearingTitle = await GetHearingTitle(hearing);
                result = result.Replace("{{HearingTitle}}", hearingTitle);
            }

            if (result.Contains("{{LinkToHearing}}"))
            {
                var hearingLink = GetLinkToHearing(hearing);
                result = result.Replace("{{LinkToHearing}}", hearingLink);
            }

            if (result.Contains("{{TermsAndConditions}}"))
            {
                var termsAndConditions = await GetTermsAndConditionsText();
                result = result.Replace("{{TermsAndConditions}}", termsAndConditions);
            }

            if (result.Contains("{{Municipality}}"))
            {
                var municipality = GetMunicipality();
                result = result.Replace("{{Municipality}}", municipality);
            }

            return result;
        }

        protected List<string> ReplaceNewLineVariable(string template)
        {
            return template.Split("{{NewLine}}").ToList();
        }

        public async Task<string> GetHearingTitle(Hearing hearing)
        {
            return (await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE))?.TextContent ?? string.Empty;
        }

        public string GetLinkToHearing(Hearing hearing)
        {
            return hearing.CreateLinkToHearing(_options.Value.PathToReadInternalHearing, _options.Value.PathToReadPublicHearing) ?? string.Empty;
        }

        public async Task<string> GetTermsAndConditionsText()
        {
            var termsAndConditions = await _globalContentDao.GetLatestVersionOfTypeAsync(GlobalContentType.TERMS_AND_CONDITIONS);
            return termsAndConditions?.Content ?? string.Empty;
        }

        public string GetMunicipality()
        {
            return _textResolver.GetText(GroupKey.General, TextKey.MunicipalityName);
        }

        protected List<ContentAndType> GetNotificationContents(NotificationType notificationType)
        {
            return new List<ContentAndType>
            {
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.SUBJECT,
                    TextContent = notificationType.SubjectTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.HEADER,
                    TextContent = notificationType.HeaderTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.BODY,
                    TextContent = notificationType.BodyTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.FOOTER,
                    TextContent = notificationType.FooterTemplate.TextContent
                }
            };
        }

        protected List<ContentAndType> GetNotificationContents(NotificationType notificationType, NotificationContentSpecification notificationContentSpecification)
        {
            return new List<ContentAndType>
            {
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.SUBJECT,
                    TextContent = notificationContentSpecification.SubjectContent?.TextContent ?? notificationType.SubjectTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.HEADER,
                    TextContent = notificationContentSpecification.HeaderContent?.TextContent ?? notificationType.HeaderTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.BODY,
                    TextContent = notificationContentSpecification.BodyContent?.TextContent ?? notificationType.BodyTemplate.TextContent
                },
                new ContentAndType
                {
                    NotificationContentType = NotificationContentType.FOOTER,
                    TextContent = notificationContentSpecification.FooterContent?.TextContent ?? notificationType.FooterTemplate.TextContent
                }
            };
        }

        protected string GetContentByType(List<ContentAndType> contents, NotificationContentType contentType)
        {
            return contents?.FirstOrDefault(c => c.NotificationContentType == contentType)?.TextContent;
        }

        private async Task<NotificationContentSpecification>
            GetNotificationContentSpecificationForHearingAndNotificationType(int hearingId, int notificationTypeId)
        {
            var includes = IncludeProperties.Create<NotificationContentSpecification>(null, new List<string>
            {
                nameof(NotificationContentSpecification.SubjectContent),
                nameof(NotificationContentSpecification.HeaderContent),
                nameof(NotificationContentSpecification.BodyContent),
                nameof(NotificationContentSpecification.FooterContent)
            });

            var notificationContentSpecifications = await _notificationContentSpecificationDao.GetAllAsync(includes,
                ncs => ncs.HearingId == hearingId && ncs.NotificationTypeId == notificationTypeId);

            // Note: During a Hearings lifetime NotificationContentSpecifications may be created for a given NotificationType and Hearing
            // There can only ever exist one NotificationContentSpecification for a given NotificationType for a given Hearing
            return notificationContentSpecifications.FirstOrDefault();
        }
    }

    public class ContentAndType
    {
        public string TextContent { get; set; }
        public NotificationContentType NotificationContentType { get; set; }
    }
}