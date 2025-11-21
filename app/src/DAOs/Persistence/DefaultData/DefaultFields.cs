using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FieldEntity = Agora.Entities.Entities.FieldEntity;
using Microsoft.EntityFrameworkCore;
using FieldType = Agora.Entities.Enums.FieldType;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultFields : DefaultDataSeeder<FieldEntity>
    {
        private static async Task<List<FieldEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var titleFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.TITLE);
            var esdhTitleFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.ESDH_TITLE);
            var imageFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.IMAGE);
            var summaryFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.SUMMARY);
            var bodyInformationFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.BODYINFORMATION);
            var conclusionFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.CONCLUSION);

            var standardTemplate = await context.HearingTemplates.FirstOrDefaultAsync();

            var titleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.TITLE);
            var esdhTitleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.ESDH_TITLE);
            var imageValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.IMAGE);
            var summaryValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.SUMMARY);
            var bodyInformationValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.BODYINFORMATION);
            var conclusionValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.CONCLUSION);

            return new List<FieldEntity>
            {
                new FieldEntity
                {
                    Name = "Titel",
                    AllowTemplates = true,
                    DisplayOrder = 100,
                    FieldType = titleFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = titleValidationRule,
                },
                new FieldEntity
                {
                    Name = "ESDH Titel",
                    AllowTemplates = true,
                    DisplayOrder = 200,
                    FieldType = esdhTitleFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = esdhTitleValidationRule,
                },
                new FieldEntity
                {
                    Name = "Billede",
                    AllowTemplates = false,
                    DisplayOrder = 300,
                    FieldType = imageFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = imageValidationRule,
                },
                new FieldEntity
                {
                    Name = "Resumé",
                    AllowTemplates = false,
                    DisplayOrder = 400,
                    FieldType = summaryFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = summaryValidationRule,
                },
                new FieldEntity
                {
                    Name = "Konklusion",
                    AllowTemplates = true,
                    DisplayOrder = 500,
                    FieldType = conclusionFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = conclusionValidationRule,
                },
                new FieldEntity
                {
                    Name = "Brødtekst",
                    AllowTemplates = true,
                    DisplayOrder = 600,
                    FieldType = bodyInformationFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = bodyInformationValidationRule,
                }
            };
        }

        private static Func<FieldEntity, FieldEntity, bool> comparer = (e1, e2) => (e1.FieldType.Type == e2.FieldType.Type && e1.HearingTemplate?.Id == e2.HearingTemplate?.Id);

        public DefaultFields(ApplicationDbContext context, List<FieldEntity> defaultEntities)
            : base(context, context.Fields, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<FieldEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultFields(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<FieldEntity> GetUpdatedEntities(List<FieldEntity> existingEntities, List<FieldEntity> defaultEntities)
        {
            var updatedEntities = new List<FieldEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));

                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.AllowTemplates = defaultEntity.AllowTemplates;
                entity.FieldType = defaultEntity.FieldType;
                entity.DisplayOrder = defaultEntity.DisplayOrder;
                entity.HearingTemplate = defaultEntity.HearingTemplate;
                entity.ValidationRule = defaultEntity.ValidationRule;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
