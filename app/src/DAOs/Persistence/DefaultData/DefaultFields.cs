using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultFields : DefaultDataSeeder<FieldEntity>
    {
        private static async Task<List<FieldEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var titleFieldType = await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.TITLE);
            var esdhTitleFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.ESDH_TITLE);
            var imageFieldType = await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.IMAGE);
            var summaryFieldType = await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.SUMMARY);
            var bodyInformationFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.BODYINFORMATION);
            var conclusionFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(fieldType => fieldType.Type == FieldType.CONCLUSION);

            var standardTemplate = await context.HearingTemplates.FirstOrDefaultAsync();

            var titleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule => vRule.FieldType == FieldType.TITLE);
            var esdhTitleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule => vRule.FieldType == FieldType.ESDH_TITLE);
            var imageValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule => vRule.FieldType == FieldType.IMAGE);
            var summaryValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule => vRule.FieldType == FieldType.SUMMARY);
            var bodyInformationValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule => vRule.FieldType == FieldType.BODYINFORMATION);
            var conclusionValidationRule = await context.ValidationRules.FirstOrDefaultAsync(vRule  => vRule.FieldType == FieldType.CONCLUSION);

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
                    Name = "Brød tekst",
                    AllowTemplates = true,
                    DisplayOrder = 600,
                    FieldType = bodyInformationFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = bodyInformationValidationRule,
                }
            };
        }

        private static Func<FieldEntity, FieldEntity, bool> comparer = (e1, e2) => (e1.Name == e2.Name && e1.FieldType.Type == e2.FieldType.Type);

        public DefaultFields(ApplicationDbContext context, List<FieldEntity> defaultEntities)
            : base(context, context.Fields, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultFields(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<FieldEntity> FetchEntitiesToUpdate(List<FieldEntity> existingEntities, List<FieldEntity> defaultEntities)
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
