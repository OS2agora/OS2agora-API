using Agora.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Persistence.DefaultData
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
                    FieldType = Entities.Enums.FieldType.TITLE,
                },
                // Esdh Titel
                new ValidationRuleEntity
                {
                    MaxLength = 110,
                    FieldType = Entities.Enums.FieldType.ESDH_TITLE,
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
                    MaxFileSize = (int)Math.Pow(10, 6), // 1 MB
                    CanBeEmpty = true,
                    FieldType = Entities.Enums.FieldType.IMAGE,
                },
                // Resumé
                new ValidationRuleEntity
                {
                    MaxLength = 500,
                    FieldType = Entities.Enums.FieldType.SUMMARY,
                },
                // Brød tekst
                new ValidationRuleEntity
                {
                    MaxFileSize = 100 * (int)Math.Pow(10, 6), // 100 MB,
                    FieldType = Entities.Enums.FieldType.BODYINFORMATION,
                    AllowedFileTypes = new[]
                    {
                        "image/jpeg",
                        "image/png",
                        "image/svg",
                        "application/pdf"
                    },
                },
                // Konklusion
                new ValidationRuleEntity
                {
                    MaxFileSize = 100 * (int)Math.Pow(10, 6), // 100 MB
                    FieldType = Entities.Enums.FieldType.CONCLUSION,
                    AllowedFileTypes = new[]
                    {
                        "image/jpeg",
                        "image/png",
                        "image/svg",
                        "application/pdf"
                    },
                }
            };
        }

        private static Func<ValidationRuleEntity, ValidationRuleEntity, bool> comparer = (e1, e2) => (e1.FieldType == e2.FieldType);

        public DefaultValidationRules(ApplicationDbContext context, List<ValidationRuleEntity> defaultEntities)
            : base(context, context.ValidationRules, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<ValidationRuleEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities(context);
            var seeder = new DefaultValidationRules(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<ValidationRuleEntity> GetUpdatedEntities(List<ValidationRuleEntity> existingEntities, List<ValidationRuleEntity> defaultEntities)
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
