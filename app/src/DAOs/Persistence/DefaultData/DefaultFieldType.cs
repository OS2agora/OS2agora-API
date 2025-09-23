using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultFieldType : DefaultDataSeeder<FieldTypeEntity>
    {
        private static List<FieldTypeEntity> GetDefaultEntities()
        {
            return new List<FieldTypeEntity>
            {
                new FieldTypeEntity
                {
                    Type = FieldType.TITLE
                },
                new FieldTypeEntity
                {
                    Type = FieldType.ESDH_TITLE
                },
                new FieldTypeEntity
                {
                    Type = FieldType.IMAGE
                },
                new FieldTypeEntity
                {
                    Type = FieldType.SUMMARY
                },
                new FieldTypeEntity
                {
                    Type = FieldType.BODYINFORMATION
                },
                new FieldTypeEntity
                {
                    Type = FieldType.CONCLUSION
                }
            };
        }

        private static Func<FieldTypeEntity, FieldTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultFieldType(ApplicationDbContext context, List<FieldTypeEntity> defaultEntities)
            : base(context, context.FieldTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultFieldType(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<FieldTypeEntity> FetchEntitiesToUpdate(List<FieldTypeEntity> existingEntities, List<FieldTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<FieldTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Type = defaultEntity.Type;
                
                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
