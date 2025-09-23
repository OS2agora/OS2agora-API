using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultUserCapacity : DefaultDataSeeder<UserCapacityEntity>
    {
        private static List<UserCapacityEntity> GetDefaultEntities()
        {
            return new List<UserCapacityEntity>
            {
                new UserCapacityEntity
                {
                    Capacity = UserCapacity.CITIZEN
                },
                new UserCapacityEntity
                {
                    Capacity = UserCapacity.EMPLOYEE
                },
                new UserCapacityEntity
                {
                    Capacity = UserCapacity.COMPANY
                }
            };  
        }

        private static Func<UserCapacityEntity, UserCapacityEntity, bool> comparer = (e1, e2) => (e1.Capacity == e2.Capacity);

        public DefaultUserCapacity(ApplicationDbContext context, List<UserCapacityEntity> defaultEntities)
            : base(context, context.UserCapacities, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultUserCapacity(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<UserCapacityEntity> FetchEntitiesToUpdate(List<UserCapacityEntity> existingEntities, List<UserCapacityEntity> defaultEntities)
        {
            var updatedEntities = new List<UserCapacityEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Capacity = defaultEntity.Capacity;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
