using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
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