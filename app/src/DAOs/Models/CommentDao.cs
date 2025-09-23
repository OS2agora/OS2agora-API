using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class CommentDao : BaseDao<CommentEntity, Comment>, ICommentDao
    {
        public CommentDao(IApplicationDbContext db, ILogger<BaseDao<CommentEntity, Comment>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Comment> GetAsync(int id, IncludeProperties includes = null)
        {
            var commentEntity = await base.GetAsync(id, includes);
            return MapAndPrune(commentEntity, includes);
        }

        public async Task<List<Comment>> GetAllAsync(IncludeProperties includes = null, List<int> hearingIds = null)
        {
            Expression<Func<CommentEntity, bool>> filter = null;
            if (hearingIds != null)
            {
                filter = commentEntity => hearingIds.Contains(commentEntity.HearingId);
            }
            var commentEntities = await base.GetAllAsync(includes, filter);
            return commentEntities.Select(commentEntity => MapAndPrune(commentEntity, includes)).ToList();
        }

        public new async Task<Comment> CreateAsync(Comment model, IncludeProperties includes = null)
        {
            var commentEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(commentEntity, includes);
        }

        public async Task<Comment> UpdateAsync(Comment model, IncludeProperties includes = null)
        {
            var commentEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(commentEntity, includes);
        }
    }
}