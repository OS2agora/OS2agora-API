using BallerupKommune.Entities.Entities;
using BallerupKommune.Operations.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultNotificationTemplates : DefaultDataSeeder<NotificationTemplateEntity>
    {
        private static List<NotificationTemplateEntity> GetDefaultEntities(ApplicationDbContext context)
        {
            return new List<NotificationTemplateEntity>
            {
                new NotificationTemplateEntity {
                    NotificationTemplateText =
                            @"<li>Reviewer: Der er tilføjet følgende reviewere: <ul><li>{{Reviewers}}</li></ul></li>",
                    SubjectTemplate = NotificationSubjectTemplates.AddedReviewers
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"<li>Høringsejer: Høringen har fået en ny høringsejer: <strong>{{HearingOwner}}.</strong></li>",
                    SubjectTemplate = NotificationSubjectTemplates.NewHearingOwner
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"<li>Statusskifte: Høringen har skiftet status til: <strong>{{HearingStatus}}.</strong></li>",
                    SubjectTemplate = NotificationSubjectTemplates.HearingStatusChange
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"<li>Høringssvar: Der er modtaget <strong>{{HearingResponseCount}}</strong> høringssvar.</li>",
                    SubjectTemplate = NotificationSubjectTemplates.NewHearingAnswers
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"<li>Høringskommentarer: Der er modtaget <strong>{{HearingReviewCount}}</strong> høringskommentarer.</li>",
                    SubjectTemplate = NotificationSubjectTemplates.NewHearingComments
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"{{HearingTitle}}{{NewLine}}{{NewLine}}Vi inviterer dig til at deltage i denne høring.{{NewLine}}Du kan se detaljerne om høringen og indsende dit høringssvar på dette link: {{LinkToHearing}}{{NewLine}}Vær opmærksom på at du skal logge ind med MitId.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                    SubjectTemplate = NotificationSubjectTemplates.PublicHearing
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"Dit høringssvar{{CompanyResponder}} er blevet afvist på høringen: {{HearingTitle}}.{{NewLine}}{{NewLine}}Dette kan være sket fordi dit svar enten vurderes til ikke at høre til den aktuelle høring, eller at det indeholdt informationer og/eller persondata som vurderes ikke at være lovlige at offentliggøre i en høringssammenhæng.{{NewLine}}{{NewLine}}Nedenstående er høringsadministratorens begrundelse for afvisning af høringssvar {{CommentNumber}}:{{CommentDeclinedReason}}{{NewLine}}{{NewLine}}Det fulde svar indgår dog i den videre behandling.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                    SubjectTemplate = NotificationSubjectTemplates.HearingAnswerDeclined
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"Der foreligger nu en konklusion på høringen: {{HearingTitle}}, som du har svaret på.{{NewLine}}{{NewLine}}Du kan se konklusionen på høringen her: {{LinkToHearing}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                    SubjectTemplate = NotificationSubjectTemplates.HearingConcluded
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                        @"Der er foretaget en ændring på høringen: {{HearingTitle}}, som du har svaret på.{{NewLine}}{{NewLine}}Der er tale om en mindre rettelse, men dette kan stadig have indflydelse på dit afgivne svar.{{NewLine}} Du kan se den opdaterede høringstekst samt rette dit svar her: {{LinkToHearing}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                    SubjectTemplate = NotificationSubjectTemplates.HearingUpdated
                },
                new NotificationTemplateEntity
                {
                    NotificationTemplateText =
                            @"Tak for dit høringssvar til høringen: {{HearingTitle}}.{{NewLine}}{{NewLine}}Svaret{{CompanyResponder}} indgår nu i det videre arbejde med høringen.{{NewLine}}Hvis dit svar indeholder følsomme personoplysninger, forbeholder vi os ret til ikke at publicere dit svar på høringsportalen.{{NewLine}}Det fulde svar indgår dog i den videre behandling.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                    SubjectTemplate = NotificationSubjectTemplates.HearingAnswerConfirmation
                }
            };
        }

        private static Func<NotificationTemplateEntity, NotificationTemplateEntity, bool> comparer = (e1, e2) => (e1.SubjectTemplate == e2.SubjectTemplate && e1.NotificationTemplateText.Length == e2.NotificationTemplateText.Length);

        public DefaultNotificationTemplates(ApplicationDbContext context, List<NotificationTemplateEntity> defaultEntities)
            : base(context, context.NotificationTemplates, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities(context);
            var seeder = new DefaultNotificationTemplates(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<NotificationTemplateEntity> FetchEntitiesToUpdate(List<NotificationTemplateEntity> existingEntities, List<NotificationTemplateEntity> defaultEntities)
        {
            var updatedEntities = new List<NotificationTemplateEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.NotificationTemplateText = defaultEntity.NotificationTemplateText;
                entity.SubjectTemplate = defaultEntity.SubjectTemplate;
                
                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
