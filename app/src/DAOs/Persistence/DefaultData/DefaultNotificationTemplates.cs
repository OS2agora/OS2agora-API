using Agora.Entities.Entities;
using Agora.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultNotificationTemplates : DefaultDataSeeder<NotificationTemplateEntity>
    {
        public const string DefaultFooter = "Default footer";
        
        public const string AddedAsReviewerSubject = "Default tilføjet som reviewer subject";
        public const string AddedAsReviewerHeader = "Default tilføjet som reviewer header";
        public const string AddedAsReviewerBody = "Default tilføjet som reviewer body";

        public const string InvitedToHearingSubject = "Default invitations subject";
        public const string InvitedToHearingHeader = "Default invitations header";
        public const string InvitedToHearingBody = "Default invitations body";
        public const string InvitedToHearingFooter = "Default invitations footer";

        public const string ReceiptSubject = "Default kvitterings subject";
        public const string ReceiptHeader = "Default kvitterings header";
        public const string ReceiptBody = "Default kvitterings body";

        public const string ConcludedSubject = "Default høring konkluderet subject";
        public const string ConcludedHeader = "Default høring konkluderet header";
        public const string ConcludedBody = "Default høring konkluderet body";
        
        public const string UpdatedSubject = "Default høring opdateret subject";
        public const string UpdatedHeader = "Default høring opdateret header";
        public const string UpdatedBody = "Default høring opdateret body";
        
        public const string RejectedSubject = "Default svar afvist subject";
        public const string RejectedHeader = "Default svar afvist header";
        public const string RejectedBody = "Default svar afvist body";
        
        public const string DailyStatusSubject = "Default daglig status subject";
        public const string DailyStatusHeader = "Default daglig status header";
        public const string DailyStatusBody = "Default daglig status body";
        
        public const string NewsLetterSubject = "Default nyhedsbrev subject";
        public const string NewsLetterHeader = "Default nyhedsbrev header";
        public const string NewsLetterBody = "Default nyhedsbrev body";


        private static async Task<List<NotificationTemplateEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var notificationContentTypes = await context.NotificationContentTypes.ToListAsync();
            var subjectNotificationContentTypeId = notificationContentTypes.SingleOrDefault(nct => nct.Type == NotificationContentType.SUBJECT)!.Id;
            var headerNotificationContentTypeId = notificationContentTypes.SingleOrDefault(nct => nct.Type == NotificationContentType.HEADER)!.Id;
            var bodyNotificationContentTypeId = notificationContentTypes.SingleOrDefault(nct => nct.Type == NotificationContentType.BODY)!.Id;
            var footerNotificationContentTypeId = notificationContentTypes.SingleOrDefault(nct => nct.Type == NotificationContentType.FOOTER)!.Id;

            return new List<NotificationTemplateEntity>
            {
                new NotificationTemplateEntity {
                    Name = DefaultFooter,
                    NotificationContentTypeId = footerNotificationContentTypeId,
                    TextContent = "{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}{{Municipality}}"
                },
                new NotificationTemplateEntity {
                    Name = AddedAsReviewerSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Tilføjet som reviewer"
                },
                new NotificationTemplateEntity {
                    Name = AddedAsReviewerHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Du er blevet tilføjet som reviewer på høringen {{HearingTitle}}."
                },
                new NotificationTemplateEntity {
                    Name = AddedAsReviewerBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = "Som reviewer vil du dagligt modtage en opdatering, hvis der har været aktivitet på høringen."
                },
                new NotificationTemplateEntity {
                    Name = InvitedToHearingSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Inviteret til høring"
                },
                new NotificationTemplateEntity {
                    Name = InvitedToHearingHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Vi inviterer dig til at deltage i denne høring: {{HearingTitle}}{{NewLine}}Du kan se detaljerne om høringen og indsende dit høringssvar på dette link: {{LinkToHearing}}"
                },
                new NotificationTemplateEntity {
                    Name = InvitedToHearingBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = string.Empty // TODO: Should be updated
                },
                new NotificationTemplateEntity {
                    Name = InvitedToHearingFooter,
                    NotificationContentTypeId = footerNotificationContentTypeId,
                    TextContent = "Vær opmærksom på at du skal logge ind med MitId.{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}{{Municipality}}"
                },
                new NotificationTemplateEntity {
                    Name = ReceiptSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Høringsvar modtaget"
                },
                new NotificationTemplateEntity {
                    Name = ReceiptHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Tak for dit svar til høringen '{{HearingTitle}}'. Svaret{{CompanyResponder}} indgår nu i det videre arbejde med høringen."
                },
                new NotificationTemplateEntity {
                    Name = ReceiptBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = "Hvis dit svar indeholder følsomme personoplysninger, forbeholder vi os ret til ikke at publicere dit svar på høringsportalen.{{NewLine}}Det fulde svar indgår dog i den videre behandling."
                },
                new NotificationTemplateEntity {
                    Name = ConcludedSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Høring konkluderet"
                },
                new NotificationTemplateEntity {
                    Name = ConcludedHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Der foreligger nu en konklusion på høringen: {{HearingTitle}}, som du er inviteret til at følge.{{NewLine}}{{NewLine}}Du kan se konklusionen på høringen her: {{LinkToHearing}}"
                },
                new NotificationTemplateEntity {
                    Name = ConcludedBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = string.Empty // TODO: Should be updated
                },
                new NotificationTemplateEntity {
                    Name = UpdatedSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Høring opdateret"
                },
                new NotificationTemplateEntity {
                    Name = UpdatedHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Der er foretaget en ændring på høringen '{{HearingTitle}}', som du er inviteret til at følge.{{NewLine}}Du kan se den opdaterede høringstekst samt rette dit svar her: {{LinkToHearing}}"
                },
                new NotificationTemplateEntity {
                    Name = UpdatedBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = "Der er tale om en mindre rettelse, men dette kan stadig have indflydelse på dit afgivne svar."
                },
                new NotificationTemplateEntity {
                    Name = RejectedSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Høringssvar afvist"
                },
                new NotificationTemplateEntity {
                    Name = RejectedHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Dit høringssvar{{CompanyResponder}} er blevet afvist på høringen: {{HearingTitle}}.{{NewLine}}Nedenstående er høringsadministratorens begrundelse for afvisning af høringssvar {{CommentNumber}}:{{CommentDeclinedReason}}"
                },
                new NotificationTemplateEntity {
                    Name = RejectedBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = "Dette kan være sket fordi dit svar enten vurderes til ikke at høre til den aktuelle høring, eller at det indeholdt informationer og/eller persondata som vurderes ikke at være lovlige at offentliggøre i en høringssammenhæng.{{NewLine}}{{NewLine}}Det fulde svar indgår dog i den videre behandling."
                },
                new NotificationTemplateEntity {
                    Name = DailyStatusSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Daglig status"
                },
                new NotificationTemplateEntity {
                    Name = DailyStatusHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = string.Empty
                },
                new NotificationTemplateEntity {
                    Name = DailyStatusBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = string.Empty
                },
                new NotificationTemplateEntity {
                    Name = NewsLetterSubject,
                    NotificationContentTypeId = subjectNotificationContentTypeId,
                    TextContent = "Nyhedsbrev"
                },
                new NotificationTemplateEntity {
                    Name = NewsLetterHeader,
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = string.Empty
                },
                new NotificationTemplateEntity {
                    Name = NewsLetterBody,
                    NotificationContentTypeId = bodyNotificationContentTypeId,
                    TextContent = string.Empty
                }
            };
        }

        private static Func<NotificationTemplateEntity, NotificationTemplateEntity, bool> comparer = (e1, e2) => (e1.Name == e2.Name);

        public DefaultNotificationTemplates(ApplicationDbContext context, List<NotificationTemplateEntity> defaultEntities)
            : base(context, context.NotificationTemplates, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<NotificationTemplateEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultNotificationTemplates(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<NotificationTemplateEntity> GetUpdatedEntities(List<NotificationTemplateEntity> existingEntities, List<NotificationTemplateEntity> defaultEntities)
        {
            var updatedEntities = new List<NotificationTemplateEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.NotificationContentTypeId = defaultEntity.NotificationContentTypeId;
                entity.TextContent = defaultEntity.TextContent;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
