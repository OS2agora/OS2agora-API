using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public class DefaultCommentType : DefaultDataSeeder<CommentTypeEntity>
    {
        private static List<CommentTypeEntity> GetDefaultEntities()
        {
            return new List<CommentTypeEntity>
            {
                new CommentTypeEntity
                {
                    Type = CommentType.HEARING_RESPONSE
                },
                new CommentTypeEntity
                {
                    Type = CommentType.HEARING_REVIEW
                },
                new CommentTypeEntity
                {
                    Type = CommentType.HEARING_RESPONSE_REPLY
                }
            };  
        }

        private static Func<CommentTypeEntity, CommentTypeEntity, bool> comparer = (e1, e2) => (e1.Type == e2.Type);

        public DefaultCommentType(ApplicationDbContext context, List<CommentTypeEntity> defaultEntities)
            : base(context, context.CommentTypes, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context)
        {
            var defaultEntities = GetDefaultEntities();
            var seeder = new DefaultCommentType(context, defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<CommentTypeEntity> FetchEntitiesToUpdate(List<CommentTypeEntity> existingEntities, List<CommentTypeEntity> defaultEntities)
        {
            var updatedEntities = new List<CommentTypeEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));
                if (defaultEntity == null)
                {
                    continue;
                }

                entity.Type = defaultEntity.Type;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
