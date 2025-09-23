using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultValidationRules : DefaultDataSeeder<ValidationRuleEntity>
    {
        private static List<ValidationRuleEntity> GetDefaultEntities(ApplicationDbContext context)
        {
            return new List<ValidationRuleEntity>
            {
                // Titel
                new ValidationRuleEntity
                {
                    CanBeEmpty = false,
                    MaxLength = 60,
                    FieldType = FieldType.TITLE,
                },
                // Esdh Titel
                new ValidationRuleEntity
                {
                    CanBeEmpty = false,
                    MaxLength = 110,
                    FieldType = FieldType.ESDH_TITLE,
                },
                // Billede
                new ValidationRuleEntity
                {
                    AllowedFileTypes = new[]
                    {
                        "image/jpeg",
                        "image/png",
                        "image/svg"
                    },
                    MaxFileSize = 1000000,
                    CanBeEmpty = true,
                    FieldType = FieldType.IMAGE,
                },
                // Resumé
                new ValidationRuleEntity
                {
                    MaxLength = 500,
                    FieldType = FieldType.SUMMARY,
                },
                // Brød tekst
                new ValidationRuleEntity
                {
                    MaxFileSize = 100000000,
                    FieldType = FieldType.BODYINFORMATION,
                },
                new ValidationRuleEntity
                {
                    MaxLength = 1000,
                    CanBeEmpty = false,
                    FieldType = FieldType.CONCLUSION,
                }
            };
        }

        private static Func<ValidationRuleEntity, ValidationRuleEntity, bool> comparer = (e1, e2) => (e1.FieldType == e2.FieldType);

        public DefaultValidationRules(ApplicationDbContext context, List<ValidationRuleEntity> defaultEntities)
            : base(context, context.ValidationRules, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities(context);
            var seeder = new DefaultValidationRules(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<ValidationRuleEntity> FetchEntitiesToUpdate(List<ValidationRuleEntity> existingEntities, List<ValidationRuleEntity> defaultEntities)
        {
            var updatedEntities = new List<ValidationRuleEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.FieldType = defaultEntity.FieldType;
                entity.MaxLength = defaultEntity.MaxLength;
                entity.MinLength = defaultEntity.MinLength;
                entity.MaxFileCount = defaultEntity.MaxFileCount;
                entity.MaxFileSize = defaultEntity.MaxFileSize;
                entity.AllowedFileTypes = defaultEntity.AllowedFileTypes;
                entity.CanBeEmpty = defaultEntity.CanBeEmpty;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
