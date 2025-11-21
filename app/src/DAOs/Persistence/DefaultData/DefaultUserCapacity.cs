using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserCapacityEntity = Agora.Entities.Entities.UserCapacityEntity;
using UserCapacity = Agora.Entities.Enums.UserCapacity;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
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

        private static readonly Func<UserCapacityEntity, UserCapacityEntity, bool> _comparer = (e1, e2) => (e1.Capacity == e2.Capacity);

        public DefaultUserCapacity(ApplicationDbContext context, List<UserCapacityEntity> defaultEntities)
            : base(context, context.UserCapacities, defaultEntities, _comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<UserCapacityEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultUserCapacity(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<UserCapacityEntity> GetUpdatedEntities(List<UserCapacityEntity> existingEntities, List<UserCapacityEntity> defaultEntities)
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
