using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingStatus = Agora.Entities.Enums.HearingStatus;
using NotificationType = Agora.Entities.Enums.NotificationType;

namespace Agora.DAOs.Persistence
{
    public static class ApplicationDbContextEnsureRequiredData
    {
        public static async Task SeedRequiredData(ApplicationDbContext context)
        {
            await EnsureInvitationContentSpecificationOfTypeExistForHearings(context);
            await EnsureConcludedContentSpecificationOfTypeExistForHearings(context);
        }

        private static async Task EnsureInvitationContentSpecificationOfTypeExistForHearings(ApplicationDbContext context)
        {
            var createdHearingStatus = await context.HearingStatus.AsNoTracking()
                .SingleAsync(hearingStatus => hearingStatus.Status == HearingStatus.CREATED);

            var invitationNotificationType = await GetNotificationTypeEntityOfType(context, NotificationType.INVITED_TO_HEARING);

            var hearingIdsToUpdate = await
                context.Hearings
                    .Where(hearing => hearing.HearingStatusId != createdHearingStatus.Id)
                    .Where(hearing => hearing.NotificationContentSpecifications.All(ncs => ncs.NotificationTypeId != invitationNotificationType.Id))
                    .Select(hearing => hearing.Id)
                    .ToListAsync();

            if (hearingIdsToUpdate.Count == 0)
            {
                return;
            }

            await AddNotificationContentSpecificationsForHearings(context, hearingIdsToUpdate, invitationNotificationType);
        }

        private static async Task EnsureConcludedContentSpecificationOfTypeExistForHearings(ApplicationDbContext context)
        {
            var awaitingConclusionHearingStatus = await context.HearingStatus.AsNoTracking()
                .SingleAsync(hearingStatus => hearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
            var concludedHearingStatus = await context.HearingStatus.AsNoTracking()
                .SingleAsync(hearingStatus => hearingStatus.Status == HearingStatus.CONCLUDED);

            var conclusionNotificationType = await GetNotificationTypeEntityOfType(context, NotificationType.HEARING_CONCLUSION_PUBLISHED);

            var hearingIdsToUpdate = await
                context.Hearings
                    .Where(hearing => hearing.HearingStatusId == awaitingConclusionHearingStatus.Id || hearing.HearingStatusId == concludedHearingStatus.Id)
                    .Where(hearing => hearing.NotificationContentSpecifications.All(ncs => ncs.NotificationTypeId != conclusionNotificationType.Id))
                    .Select(hearing => hearing.Id)
                    .ToListAsync();

            if (hearingIdsToUpdate.Count == 0)
            {
                return;
            }

            await AddNotificationContentSpecificationsForHearings(context, hearingIdsToUpdate, conclusionNotificationType);
        }

        private static async Task<NotificationTypeEntity> GetNotificationTypeEntityOfType(ApplicationDbContext context, NotificationType notificationType)
        {
            return await context.NotificationTypes
                .Include(nt => nt.SubjectTemplate)
                .Include(nt => nt.HeaderTemplate)
                .Include(nt => nt.BodyTemplate)
                .Include(nt => nt.FooterTemplate)
                .SingleAsync(nt => nt.Type == notificationType);
        }

        private static async Task AddNotificationContentSpecificationsForHearings(ApplicationDbContext context,
            List<int> hearingIds, NotificationTypeEntity notificationType)
        {
            var notificationContentSpecificationsToCreate = hearingIds.Select(hearingId =>
                new NotificationContentSpecificationEntity
                {
                    HearingId = hearingId,
                    NotificationTypeId = notificationType.Id,
                    SubjectContent = new NotificationContentEntity
                    {
                        TextContent = notificationType.SubjectTemplate.TextContent,
                        NotificationContentTypeId = notificationType.SubjectTemplate.NotificationContentTypeId
                    },
                    HeaderContent = new NotificationContentEntity
                    {
                        TextContent = notificationType.HeaderTemplate.TextContent,
                        NotificationContentTypeId = notificationType.HeaderTemplate.NotificationContentTypeId
                    },
                    BodyContent = new NotificationContentEntity
                    {
                        TextContent = notificationType.BodyTemplate.TextContent,
                        NotificationContentTypeId = notificationType.BodyTemplate.NotificationContentTypeId
                    },
                    FooterContent = new NotificationContentEntity
                    {
                        TextContent = notificationType.FooterTemplate.TextContent,
                        NotificationContentTypeId = notificationType.FooterTemplate.NotificationContentTypeId
                    }
                }).ToList();

            const int batchSize = 500;
            for (var i = 0; i < notificationContentSpecificationsToCreate.Count; i += batchSize)
            {
                var batch = notificationContentSpecificationsToCreate.Skip(i).Take(batchSize);
                await context.NotificationContentSpecifications.AddRangeAsync(batch);
                await context.SaveChangesAsync();
            }
        }
    }
}