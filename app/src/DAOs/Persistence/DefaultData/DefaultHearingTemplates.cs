using BallerupKommune.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultHearingTemplate : DefaultDataSeeder<HearingTemplateEntity>
    {
        private static List<HearingTemplateEntity> GetDefaultEntities()
        {
            return new List<HearingTemplateEntity>
            {
                new HearingTemplateEntity
                {
                    Name = "Standard høringsskabelon"
                }
            };
        }

        private static Func<HearingTemplateEntity, HearingTemplateEntity, bool> comparer = (e1, e2) => (e1.Name == e2.Name);

        public DefaultHearingTemplate(ApplicationDbContext context, List<HearingTemplateEntity> defaultEntities)
            : base(context, context.HearingTemplates, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultHearingTemplate(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<HearingTemplateEntity> FetchEntitiesToUpdate(List<HearingTemplateEntity> existingEntities, List<HearingTemplateEntity> defaultEntities)
        {
            var updatedEntities = new List<HearingTemplateEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
