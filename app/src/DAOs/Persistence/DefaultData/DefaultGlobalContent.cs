using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalContentEntity = Agora.Entities.Entities.GlobalContentEntity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Agora.Entities.Enums;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultGlobalContent : DefaultDataSeeder<GlobalContentEntity>
    {
        private static async Task<List<GlobalContentEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var termsAndConditionsType =
                await context.GlobalContentTypes.FirstOrDefaultAsync(x => x.Type == GlobalContentType.TERMS_AND_CONDITIONS);
            var cookieInformationType =
                await context.GlobalContentTypes.FirstOrDefaultAsync(x => x.Type == GlobalContentType.COOKIE_INFORMATION);

            return new List<GlobalContentEntity> {
                new GlobalContentEntity
                {
                    Content = "Udfyld venligst denne tekst vedrørende vilkår og betingelser via administrations siden",
                    Version = 1,
                    GlobalContentType = termsAndConditionsType
                },
                new GlobalContentEntity
                {
                Content = "Udfyld venligst denne tekst vedrørende cookies via administrations siden",
                Version = 1,
                GlobalContentType = cookieInformationType
            }
            };
        }

        private static Func<GlobalContentEntity, GlobalContentEntity, bool> comparer = (e1, e2) => (e1.GlobalContentType == e2.GlobalContentType);

        public DefaultGlobalContent(ApplicationDbContext context, List<GlobalContentEntity> defaultEntities)
            : base(context, context.GlobalContents, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<GlobalContentEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = await GetDefaultEntities(context);

            var seeder = new DefaultGlobalContent(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<GlobalContentEntity> GetUpdatedEntities(List<GlobalContentEntity> existingEntities, List<GlobalContentEntity> defaultEntities)
        {
            return new List<GlobalContentEntity>();
        }
    }
}
