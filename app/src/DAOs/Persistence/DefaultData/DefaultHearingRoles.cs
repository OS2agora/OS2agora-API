using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HearingRoleEntity = Agora.Entities.Entities.HearingRoleEntity;
using HearingRole = Agora.Entities.Enums.HearingRole;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultHearingRoles : DefaultDataSeeder<HearingRoleEntity>
    {
        private static List<HearingRoleEntity> GetDefaultEntities()
        {
            return new List<HearingRoleEntity>
            {
                new HearingRoleEntity
                {
                    Name = "Høringsejer",
                    Role = HearingRole.HEARING_OWNER
                },
                new HearingRoleEntity
                {
                    Name = "Inviteret",
                    Role = HearingRole.HEARING_INVITEE
                },
                new HearingRoleEntity
                {
                    Name = "Reviewer",
                    Role = HearingRole.HEARING_REVIEWER
                },
                new HearingRoleEntity
                {
                    Name = "Besvarer",
                    Role = HearingRole.HEARING_RESPONDER
                }
            };
        }

        private static Func<HearingRoleEntity, HearingRoleEntity, bool> comparer = (e1, e2) => (e1.Role == e2.Role);

        public DefaultHearingRoles(ApplicationDbContext context, List<HearingRoleEntity> defaultEntities)
            : base(context, context.HearingRoles, defaultEntities, comparer) 
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<HearingRoleEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultHearingRoles(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<HearingRoleEntity> GetUpdatedEntities(List<HearingRoleEntity> existingEntities, List<HearingRoleEntity> defaultEntities)
        {
            var updatedEntities = new List<HearingRoleEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));

                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Name = defaultEntity.Name;
                entity.Role = defaultEntity.Role;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
