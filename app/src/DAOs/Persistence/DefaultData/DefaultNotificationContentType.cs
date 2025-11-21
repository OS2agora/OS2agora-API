using Agora.Entities.Entities;
using Agora.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultNotificationContentType : DefaultDataSeeder<NotificationContentTypeEntity>
    {
        private static List<NotificationContentTypeEntity> GetDefaultEntities()
        {
            return new List<NotificationContentTypeEntity>
            {
                new NotificationContentTypeEntity
                {
                    Type = NotificationContentType.SUBJECT,
                    CanEdit = true
                },
                new NotificationContentTypeEntity
                {
                    Type = NotificationContentType.HEADER,
                    CanEdit = false
                },
                new NotificationContentTypeEntity
                {
                    Type = NotificationContentType.BODY,
                    CanEdit = true
                },
                new NotificationContentTypeEntity
                {
                    Type = NotificationContentType.FOOTER,
                    CanEdit = false
                }
            };
        }

        private static Func<NotificationContentTypeEntity, NotificationContentTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultNotificationContentType(ApplicationDbContext context, List<NotificationContentTypeEntity> defaultEntities)
            : base(context, context.NotificationContentTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<NotificationContentTypeEntity> municipalitySpecificEntities = null)
        {
            List<NotificationContentTypeEntity> defaultEntities = GetDefaultEntities();
            var seeder = new DefaultNotificationContentType(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<NotificationContentTypeEntity> GetUpdatedEntities(List<NotificationContentTypeEntity> existingEntities, List<NotificationContentTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<NotificationContentTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Type = defaultEntity.Type;
                entity.CanEdit = defaultEntity.CanEdit;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}