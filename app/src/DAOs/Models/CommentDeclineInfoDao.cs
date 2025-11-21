using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class CommentDeclineInfoDao : BaseDao<CommentDeclineInfoEntity, CommentDeclineInfo>, ICommentDeclineInfoDao
    {
        public CommentDeclineInfoDao(IApplicationDbContext db, ILogger<BaseDao<CommentDeclineInfoEntity, CommentDeclineInfo>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<CommentDeclineInfo> GetAsync(int id, IncludeProperties includes = null)
        {
            var commentDeclineInfoEntity = await base.GetAsync(id, includes);
            return MapAndPrune(commentDeclineInfoEntity, includes);
        }

        public new async Task<CommentDeclineInfo> CreateAsync(CommentDeclineInfo model, IncludeProperties includes = null)
        {
            var commentDeclineInfoEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(commentDeclineInfoEntity, includes);
        }

        public async Task<CommentDeclineInfo> UpdateAsync(CommentDeclineInfo model, IncludeProperties includes = null)
        {
            var commentDeclineInfoEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(commentDeclineInfoEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}
