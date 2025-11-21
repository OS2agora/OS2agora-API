using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationType = Agora.Entities.Enums.NotificationType;
using NotificationTypeEntity = Agora.Entities.Entities.NotificationTypeEntity;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultNotificationType : DefaultDataSeeder<NotificationTypeEntity>
    {
        private static async Task<List<NotificationTypeEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var notificationTemplates = await context.NotificationTemplates.ToListAsync();
            var notificationTemplateDefaultFooterId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.DefaultFooter)!.Id;

            var notificationTemplateDefaultAddedAsReviewerSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.AddedAsReviewerSubject)!.Id;
            var notificationTemplateDefaultAddedAsReviewerHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.AddedAsReviewerHeader)!.Id;
            var notificationTemplateDefaultAddedAsReviewerBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.AddedAsReviewerBody)!.Id;
            
            var notificationTemplateDefaultInvitationSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.InvitedToHearingSubject)!.Id;
            var notificationTemplateDefaultInvitationHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.InvitedToHearingHeader)!.Id;
            var notificationTemplateDefaultInvitationBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.InvitedToHearingBody)!.Id;
            var notificationTemplateDefaultInvitationFooterId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.InvitedToHearingFooter)!.Id;
            
            var notificationTemplateDefaultReceiptSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ReceiptSubject)!.Id;
            var notificationTemplateDefaultReceiptHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ReceiptHeader)!.Id;
            var notificationTemplateDefaultReceiptBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ReceiptBody)!.Id;
            
            var notificationTemplateDefaultHearingConclusionPublishedSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ConcludedSubject)!.Id;
            var notificationTemplateDefaultHearingConclusionPublishedHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ConcludedHeader)!.Id;
            var notificationTemplateDefaultHearingConclusionPublishedBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.ConcludedBody)!.Id;
            
            var notificationTemplateDefaultHearingChangedSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.UpdatedSubject)!.Id;
            var notificationTemplateDefaultHearingChangedHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.UpdatedHeader)!.Id;
            var notificationTemplateDefaultHearingChangedBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.UpdatedBody)!.Id;
            
            var notificationTemplateDefaultHearingResponseDeclinedSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.RejectedSubject)!.Id;
            var notificationTemplateDefaultHearingResponseDeclinedHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.RejectedHeader)!.Id;
            var notificationTemplateDefaultHearingResponseDeclinedBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.RejectedBody)!.Id;
            
            var notificationTemplateDefaultDailyStatusSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.DailyStatusSubject)!.Id;
            var notificationTemplateDefaultDailyStatusHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.DailyStatusHeader)!.Id;
            var notificationTemplateDefaultDailyStatusBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.DailyStatusBody)!.Id;
            
            var notificationTemplateDefaultNewsletterStatusSubjectId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.NewsLetterSubject)!.Id;
            var notificationTemplateDefaultNewsletterStatusHeaderId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.NewsLetterHeader)!.Id;
            var notificationTemplateDefaultNewsletterStatusBodyId = notificationTemplates.FirstOrDefault(nt => nt.Name == DefaultNotificationTemplates.NewsLetterBody)!.Id;

            return new List<NotificationTypeEntity>
            {
                new NotificationTypeEntity
                {
                    Name = "Tilføjet som reviewer",
                    Type = NotificationType.ADDED_AS_REVIEWER,
                    SubjectTemplateId = notificationTemplateDefaultAddedAsReviewerSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultAddedAsReviewerHeaderId,
                    BodyTemplateId = notificationTemplateDefaultAddedAsReviewerBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Inviteret til høring",
                    Type = NotificationType.INVITED_TO_HEARING,
                    SubjectTemplateId = notificationTemplateDefaultInvitationSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultInvitationHeaderId,
                    BodyTemplateId = notificationTemplateDefaultInvitationBodyId,
                    FooterTemplateId = notificationTemplateDefaultInvitationFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Kvittering for høringssvar",
                    Type = NotificationType.HEARING_ANSWER_RECEIPT,
                    SubjectTemplateId = notificationTemplateDefaultReceiptSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultReceiptHeaderId,
                    BodyTemplateId = notificationTemplateDefaultReceiptBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Høringskonklusion publiseret",
                    Type = NotificationType.HEARING_CONCLUSION_PUBLISHED,
                    SubjectTemplateId = notificationTemplateDefaultHearingConclusionPublishedSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultHearingConclusionPublishedHeaderId,
                    BodyTemplateId = notificationTemplateDefaultHearingConclusionPublishedBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Høring ændret",
                    Type = NotificationType.HEARING_CHANGED,
                    SubjectTemplateId = notificationTemplateDefaultHearingChangedSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultHearingChangedHeaderId,
                    BodyTemplateId = notificationTemplateDefaultHearingChangedBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Høringssvar afvist",
                    Type = NotificationType.HEARING_RESPONSE_DECLINED,
                    SubjectTemplateId = notificationTemplateDefaultHearingResponseDeclinedSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultHearingResponseDeclinedHeaderId,
                    BodyTemplateId = notificationTemplateDefaultHearingResponseDeclinedBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Daglig status",
                    Type = NotificationType.DAILY_STATUS,
                    SubjectTemplateId = notificationTemplateDefaultDailyStatusSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultDailyStatusHeaderId,
                    BodyTemplateId = notificationTemplateDefaultDailyStatusBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                },
                new NotificationTypeEntity
                {
                    Name = "Nyhedsbrev",
                    Type = NotificationType.NEWSLETTER,
                    SubjectTemplateId = notificationTemplateDefaultNewsletterStatusSubjectId,
                    HeaderTemplateId = notificationTemplateDefaultNewsletterStatusHeaderId,
                    BodyTemplateId = notificationTemplateDefaultNewsletterStatusBodyId,
                    FooterTemplateId = notificationTemplateDefaultFooterId
                }
            };
        }

        private static Func<NotificationTypeEntity, NotificationTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultNotificationType(ApplicationDbContext context, List<NotificationTypeEntity> defaultEntities)
            : base(context, context.NotificationTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<NotificationTypeEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultNotificationType(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<NotificationTypeEntity> GetUpdatedEntities(List<NotificationTypeEntity> existingEntities, List<NotificationTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<NotificationTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.Type = defaultEntity.Type;
                entity.SubjectTemplateId = defaultEntity.SubjectTemplateId;
                entity.HeaderTemplateId = defaultEntity.HeaderTemplateId;
                entity.BodyTemplateId = defaultEntity.BodyTemplateId;
                entity.FooterTemplateId = defaultEntity.FooterTemplateId;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
