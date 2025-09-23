using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultGlobalContentType : DefaultDataSeeder<GlobalContentTypeEntity>
    {
        private static List<GlobalContentTypeEntity> GetDefaultEntities()
        {
            return new List<GlobalContentTypeEntity>
            {
                new GlobalContentTypeEntity
                {
                    Name = "Vilkår og betingelser",
                    Type = GlobalContentType.TERMS_AND_CONDITIONS
                }
            };
        }

        private static Func<GlobalContentTypeEntity, GlobalContentTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultGlobalContentType(ApplicationDbContext context, List<GlobalContentTypeEntity> defaultEntities)
            : base(context, context.GlobalContentTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultGlobalContentType(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<GlobalContentTypeEntity> FetchEntitiesToUpdate(List<GlobalContentTypeEntity> existingEntities, List<GlobalContentTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<GlobalContentTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.Type = defaultEntity.Type;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
