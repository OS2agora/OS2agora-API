using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Entities.Entities;
using Agora.Entities.Enums;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultInvitationSources : DefaultDataSeeder<InvitationSourceEntity>
    {
        private static List<InvitationSourceEntity> GetDefaultEntities()
        {
            return new List<InvitationSourceEntity>
            {
                new InvitationSourceEntity
                {
                    Name = "Enkeltperson",
                    InvitationSourceType = InvitationSourceType.PERSONAL,
                    CanDeleteIndividuals = true
                },
                new InvitationSourceEntity
                {
                    Name = "Invitationsgruppe",
                    InvitationSourceType = InvitationSourceType.INVITATION_GROUP,
                    CanDeleteIndividuals = true
                },
                new InvitationSourceEntity
                {
                    Name = "CSV Fil",
                    InvitationSourceType = InvitationSourceType.CSV,
                    CanDeleteIndividuals = true,
                    CprColumnHeader = "CPR",
                    EmailColumnHeader = "Email",
                    CvrColumnHeader = "CVR"
                },
                new InvitationSourceEntity
                {
                    Name = "Excel Fil",
                    InvitationSourceType = InvitationSourceType.EXCEL,
                    CanDeleteIndividuals = true,
                    CprColumnHeader = "CPR",
                    EmailColumnHeader = "Email",
                    CvrColumnHeader = "CVR"
                }
            };
        }

        private static Func<InvitationSourceEntity, InvitationSourceEntity, bool> comparer = (e1, e2) => (e1.InvitationSourceType == e2.InvitationSourceType && e1.Name == e2.Name);

        public DefaultInvitationSources(ApplicationDbContext context, List<InvitationSourceEntity> defaultEntities) :
            base(context, context.InvitationSources, defaultEntities, comparer) { }

        public static async Task SeedData(ApplicationDbContext context,
            List<InvitationSourceEntity> municipalitySpecificEntities = null)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultInvitationSources(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<InvitationSourceEntity> GetUpdatedEntities(List<InvitationSourceEntity> existingEntities, List<InvitationSourceEntity> defaultEntities)
        {
            var updatedEntities = new List<InvitationSourceEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));

                if (defaultEntity == null)
                {
                    continue;
                }

                entity.CanDeleteIndividuals = defaultEntity.CanDeleteIndividuals;
                entity.CprColumnHeader = defaultEntity.CprColumnHeader;
                entity.EmailColumnHeader = defaultEntity.EmailColumnHeader;
                entity.CvrColumnHeader = defaultEntity.CvrColumnHeader;

                updatedEntities.Add(entity);

            }

            return updatedEntities;
        }
    }
}

