using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentStatusEntity = Agora.Entities.Entities.CommentStatusEntity;
using CommentType = Agora.Entities.Enums.CommentType;
using CommentStatus = Agora.Entities.Enums.CommentStatus;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Agora.DAOs.Persistence.DefaultData
{
    public class DefaultCommentStatus : DefaultDataSeeder<CommentStatusEntity>
    {
        private static async Task<List<CommentStatusEntity>> GetDefaultEntities(ApplicationDbContext context)
        {
            var hearingResponseCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_RESPONSE);
            var hearingReviewCommentType =
                await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_REVIEW);
            var hearingResponseReplyCommentType =
                await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_RESPONSE_REPLY);

            return new List<CommentStatusEntity>
            {
                new CommentStatusEntity
                {
                    CommentType = hearingResponseCommentType,
                    Status = CommentStatus.AWAITING_APPROVAL
                },
                new CommentStatusEntity
                {
                    CommentType = hearingResponseCommentType,
                    Status = CommentStatus.APPROVED
                },
                new CommentStatusEntity
                {
                    CommentType = hearingResponseCommentType,
                    Status = CommentStatus.NOT_APPROVED
                },
                new CommentStatusEntity
                {
                    CommentType = hearingReviewCommentType,
                    Status = CommentStatus.NONE
                },
                new CommentStatusEntity
                {
                    CommentType = hearingResponseReplyCommentType,
                    Status = CommentStatus.NONE
                }
            };
        }

        private static Func<CommentStatusEntity, CommentStatusEntity, bool> comparer = (e1, e2) => (e1.CommentType.Type == e2.CommentType.Type && e1.Status == e2.Status);

        public DefaultCommentStatus(ApplicationDbContext context, List<CommentStatusEntity> defaultEntities)
            : base(context, context.CommentStatuses, defaultEntities, comparer)
        {
        }

        public static async Task SeedData(ApplicationDbContext context, List<CommentStatusEntity> municipalitySpecificEntities = null)
        {
            List<CommentStatusEntity> defaultEntities = await GetDefaultEntities(context);
            var seeder = new DefaultCommentStatus(context, municipalitySpecificEntities ?? defaultEntities);
            await seeder.SeedEntitiesAsync();
        }

        public override List<CommentStatusEntity> GetUpdatedEntities(List<CommentStatusEntity> existingEntities, List<CommentStatusEntity> defaultEntities)
        {
            var updatedEntities = new List<CommentStatusEntity>();

            foreach (var entity in existingEntities)
            {
                var defaultEntity = defaultEntities.FirstOrDefault(e => _comparer(e, entity));

                if (defaultEntity == null)
                {
                    continue;
                }

                entity.CommentType = defaultEntity.CommentType;
                entity.Status = defaultEntity.Status;

                updatedEntities.Add(entity);
            }

            return updatedEntities;
        }
    }
}
