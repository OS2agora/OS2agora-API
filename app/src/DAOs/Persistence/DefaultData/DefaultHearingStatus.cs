using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultHearingStatus : DefaultDataSeeder<HearingStatusEntity>
    {
        private static List<HearingStatusEntity> GetDefaultEntities()
        {
            return new List<HearingStatusEntity>
            {
                new HearingStatusEntity
                {
                    Name = "Oprettet",
                    Status = HearingStatus.CREATED
                },
                new HearingStatusEntity
                {
                    Name = "Kladde",
                    Status = HearingStatus.DRAFT
                },
                new HearingStatusEntity
                {
                    Name = "Afventer startdato",
                    Status = HearingStatus.AWAITING_STARTDATE
                },
                new HearingStatusEntity
                {
                    Name = "Aktiv",
                    Status = HearingStatus.ACTIVE
                },
                new HearingStatusEntity
                {
                    Name = "Afventer konklusion",
                    Status = HearingStatus.AWAITING_CONCLUSION
                },
                new HearingStatusEntity
                {
                    Name = "Konkluderet",
                    Status = HearingStatus.CONCLUDED
                }
            };
        }

        private static Func<HearingStatusEntity, HearingStatusEntity, bool> comparer = (e1, e2) => (e1.Status == e2.Status);

        public DefaultHearingStatus(ApplicationDbContext context, List<HearingStatusEntity> defaultEntities)
            : base(context, context.HearingStatus, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultHearingStatus(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<HearingStatusEntity> FetchEntitiesToUpdate(List<HearingStatusEntity> existingEntities, List<HearingStatusEntity> defaultEntities)
        {
            var updatedEntities = new List<HearingStatusEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.Status = defaultEntity.Status;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
