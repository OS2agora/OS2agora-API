using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultGlobalContent : DefaultDataSeeder<GlobalContentEntity>
    {
        private static async Task<List<GlobalContentEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var globalContentType = await context.GlobalContentTypes.SingleAsync();

            return new List<GlobalContentEntity> {
                new GlobalContentEntity
                {
                    Content = "Udfyld venligst denne tekst via administrations siden",
                    Version = 1,
                    GlobalContentType = globalContentType
                }
            };
        }

        private static Func<GlobalContentEntity, GlobalContentEntity, bool> comparer = (e1, e2) => (e1.GlobalContentType == e2.GlobalContentType);

        public DefaultGlobalContent(ApplicationDbContext context, List<GlobalContentEntity> defaultEntities)
            : base(context, context.GlobalContents, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = await GetDefaultEntities(context);

            var seeder = new DefaultGlobalContent(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<GlobalContentEntity> FetchEntitiesToUpdate(List<GlobalContentEntity> existingEntities, List<GlobalContentEntity> defaultEntities)
        {
            var updatedEntities = new List<GlobalContentEntity>();

            return updatedEntities;
        }
    }
}
