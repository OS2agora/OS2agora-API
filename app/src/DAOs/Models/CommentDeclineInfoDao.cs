using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using AutoMapper;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using Microsoft.Extensions.Logging;

namespace BallerupKommune.DAOs.Models
{
    public class CommentDeclineInfoDao : BaseDao<CommentDeclineInfoEntity, CommentDeclineInfo>, ICommentDeclineInfoDao
    {
        public CommentDeclineInfoDao(IApplicationDbContext db, ILogger<BaseDao<CommentDeclineInfoEntity, CommentDeclineInfo>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<CommentDeclineInfo> GetAsync(int id, IncludeProperties includes = null)
        {
            var commentDeclineInfoEntity =  await base.GetAsync(id, includes);
            return MapAndPrune(commentDeclineInfoEntity, includes);
        }

        public async Task<CommentDeclineInfo> CreateAsync(CommentDeclineInfo model, IncludeProperties includes = null)
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
