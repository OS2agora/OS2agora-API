using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalContentTypeEntity = Agora.Entities.Entities.GlobalContentTypeEntity;
using GlobalContentType = Agora.Entities.Enums.GlobalContentType;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
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
                },
                new GlobalContentTypeEntity
                {
                    Name = "Cookieinformation",
                    Type = GlobalContentType.COOKIE_INFORMATION
                }

            };
        }

        private static Func<GlobalContentTypeEntity, GlobalContentTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultGlobalContentType(ApplicationDbContext context, List<GlobalContentTypeEntity> defaultEntities)
            : base(context, context.GlobalContentTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<GlobalContentTypeEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultGlobalContentType(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<GlobalContentTypeEntity> GetUpdatedEntities(List<GlobalContentTypeEntity> existingEntities, List<GlobalContentTypeEntity> defaultEntities)
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
