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
    public class CommentTypeDao : BaseDao<CommentTypeEntity, CommentType>, ICommentTypeDao
    {
        public CommentTypeDao(IApplicationDbContext db, ILogger<BaseDao<CommentTypeEntity, CommentType>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<CommentType>> GetAllAsync(IncludeProperties includes = null)
        {
            var commentTypeEntities = await base.GetAllAsync(includes);
            return commentTypeEntities.Select(commentTypeEntity => MapAndPrune(commentTypeEntity, includes)).ToList();
        }
    }
}