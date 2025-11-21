using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentTypeEntity = Agora.Entities.Entities.ContentTypeEntity;
using ContentType = Agora.Entities.Enums.ContentType;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultContentType : DefaultDataSeeder<ContentTypeEntity>
    {
        private static List<ContentTypeEntity> GetDefaultEntities()
        {
            return new List<ContentTypeEntity>
            {
                new ContentTypeEntity
                {
                    Type = ContentType.TEXT
                },
                new ContentTypeEntity
                {
                    Type = ContentType.FILE
                }
            };
        }

        private static Func<ContentTypeEntity, ContentTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultContentType(ApplicationDbContext context, List<ContentTypeEntity> defaultEntities)
            : base(context, context.ContentTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<ContentTypeEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultContentType(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<ContentTypeEntity> GetUpdatedEntities(List<ContentTypeEntity> existingEntities, List<ContentTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<ContentTypeEntity>();

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
