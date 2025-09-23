using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using BallerupKommune.Operations.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultNotificationType : DefaultDataSeeder<NotificationTypeEntity>
    {
        private static async Task<List<NotificationTypeEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var addedReviewersTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.AddedReviewers);
            var newHearingOwnerTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.NewHearingOwner);
            var hearingStatusChangeTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.HearingStatusChange);
            var newHearingAnswersTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.NewHearingAnswers);
            var newHearingCommentsTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.NewHearingComments);
            var publicHearingTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.PublicHearing);
            var hearingAnswerDeclinedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.HearingAnswerDeclined);
            var hearingAnswerConfirmationTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.HearingAnswerConfirmation);
            var hearingConcludedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template => template.SubjectTemplate == NotificationSubjectTemplates.HearingConcluded);
            var hearingUpdatedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(template  => template.SubjectTemplate == NotificationSubjectTemplates.HearingUpdated);

            return new List<NotificationTypeEntity>
            {
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.DAILY,
                    Name = "Tilføjet som reviewer",
                    Type = NotificationType.ADDED_AS_REVIEWER,
                    NotificationTemplate = addedReviewersTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.DAILY,
                    Name = "Høringsejer skiftet",
                    Type = NotificationType.CHANGED_HEARING_OWNER,
                    NotificationTemplate = newHearingOwnerTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.DAILY,
                    Name = "Høringstatus skiftet",
                    Type = NotificationType.CHANGED_HEARING_STATUS,
                    NotificationTemplate = hearingStatusChangeTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.DAILY,
                    Name = "Høringssvar modtaget",
                    Type = NotificationType.HEARING_RESPONSE_RECEIVED,
                    NotificationTemplate = newHearingAnswersTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.DAILY,
                    Name = "Høringskommentar modtaget",
                    Type = NotificationType.HEARING_REVIEW_RECEIVED,
                    NotificationTemplate = newHearingCommentsTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.INSTANT,
                    Name = "Inviteret til høring",
                    Type = NotificationType.INVITED_TO_HEARING,
                    NotificationTemplate = publicHearingTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.INSTANT,
                    Name = "Høringssvar afvist",
                    Type = NotificationType.HEARING_RESPONSE_DECLINED,
                    NotificationTemplate = hearingAnswerDeclinedTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.INSTANT,
                    Name = "Kvittering for høringssvar",
                    Type = NotificationType.HEARING_ANSWER_RECEIPT,
                    NotificationTemplate = hearingAnswerConfirmationTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.INSTANT,
                    Name = "Høringskonklusion publiseret",
                    Type = NotificationType.HEARING_CONCLUSION_PUBLISHED,
                    NotificationTemplate = hearingConcludedTemplate
                },
                new NotificationTypeEntity
                {
                    Frequency = NotificationFrequency.INSTANT,
                    Name = "Høring ændret",
                    Type = NotificationType.HEARING_CHANGED,
                    NotificationTemplate = hearingUpdatedTemplate
                }
            };
        }

        private static Func<NotificationTypeEntity, NotificationTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultNotificationType(ApplicationDbContext context, List<NotificationTypeEntity> defaultEntities)
            : base(context, context.NotificationTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            List<NotificationTypeEntity> defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultNotificationType(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<NotificationTypeEntity> FetchEntitiesToUpdate(List<NotificationTypeEntity> existingEntities, List<NotificationTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<NotificationTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Frequency = defaultEntity.Frequency;
                entity.Name = defaultEntity.Name;
                entity.Type = defaultEntity.Type;
                entity.NotificationTemplate = defaultEntity.NotificationTemplate;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
