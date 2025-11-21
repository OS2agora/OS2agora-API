using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class CommentStatusDao : BaseDao<CommentStatusEntity, CommentStatus>, ICommentStatusDao
    {
        public CommentStatusDao(IApplicationDbContext db, ILogger<BaseDao<CommentStatusEntity, CommentStatus>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<CommentStatus>> GetAllAsync(IncludeProperties includes = null)
        {
            var commentStatusEntities = await base.GetAllAsync(includes);
            return commentStatusEntities.Select(commentStatusEntity => MapAndPrune(commentStatusEntity, includes)).ToList();
        }
    }
}