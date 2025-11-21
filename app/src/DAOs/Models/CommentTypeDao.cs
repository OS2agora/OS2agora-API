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