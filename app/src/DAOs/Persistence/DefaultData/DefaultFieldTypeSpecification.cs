using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultFieldTypeSpecification : DefaultDataSeeder<FieldTypeSpecificationEntity>
    {
        private static async Task<List<FieldTypeSpecificationEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var fieldTypes = await context.FieldTypes.ToListAsync();
            var contentTypes = await context.ContentTypes.ToListAsync();

            return new List<FieldTypeSpecificationEntity>
            {
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.TITLE),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.IMAGE),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.FILE)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.IMAGE),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.SUMMARY),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.BODYINFORMATION),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.BODYINFORMATION),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.FILE)
                },
                new FieldTypeSpecificationEntity
                {
                    FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.CONCLUSION),
                    ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                }
            };
        }

        private static Func<FieldTypeSpecificationEntity, FieldTypeSpecificationEntity, bool> comparer = (e1, e2) => (e1.FieldType.Type == e2.FieldType.Type && e1.ContentType.Type == e2.ContentType.Type);

        public DefaultFieldTypeSpecification(ApplicationDbContext context, List<FieldTypeSpecificationEntity> defaultEntities)
            : base(context, context.FieldTypeSpecifications, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            List<FieldTypeSpecificationEntity> defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultFieldTypeSpecification(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<FieldTypeSpecificationEntity> FetchEntitiesToUpdate(List<FieldTypeSpecificationEntity> existingEntities, List<FieldTypeSpecificationEntity> defaultEntities)
        {
            var updatedEntities = new List<FieldTypeSpecificationEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.FieldType = defaultEntity.FieldType;
                entity.ContentType = defaultEntity.ContentType;
                
                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
