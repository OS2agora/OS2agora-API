using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultJournalizedStatus : DefaultDataSeeder<JournalizedStatusEntity>
    {
        private static List<JournalizedStatusEntity> GetDefaultEntities()
        {
            return new List<JournalizedStatusEntity>
            {
                new JournalizedStatusEntity
                {
                    Status = JournalizedStatus.NOT_JOURNALIZED
                },
                new JournalizedStatusEntity
                {
                    Status = JournalizedStatus.JOURNALIZED
                },
                new JournalizedStatusEntity
                {
                    Status = JournalizedStatus.JOURNALIZED_WITH_ERROR
                }
            };  
        }

        private static Func<JournalizedStatusEntity, JournalizedStatusEntity, bool> comparer = (e1, e2) => (e1.Status == e2.Status);

        public DefaultJournalizedStatus(ApplicationDbContext context, List<JournalizedStatusEntity> defaultEntities)
            : base(context, context.JournalizedStatuses, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultJournalizedStatus(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<JournalizedStatusEntity> FetchEntitiesToUpdate(List<JournalizedStatusEntity> existingEntities, List<JournalizedStatusEntity> defaultEntities)
        {
            var updatedEntities = new List<JournalizedStatusEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Status = defaultEntity.Status;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
