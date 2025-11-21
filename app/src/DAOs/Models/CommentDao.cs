using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Agora.DAOs.Mappings;

namespace Agora.DAOs.Models
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

        public async Task<List<Comment>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Comment, bool>> filter = null)
        {
            var commentEntities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<Comment, CommentEntity>());
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