using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class InvitationKeyDao : BaseDao<InvitationKeyEntity, InvitationKey>, IInvitationKeyDao
    {
        public InvitationKeyDao(IApplicationDbContext db, ILogger<BaseDao<InvitationKeyEntity, InvitationKey>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<InvitationKey>> CreateRangeAsync(List<InvitationKey> models, IncludeProperties includes = null)
        {
            var entities = await base.CreateRangeAsync(models, includes);
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public new async Task DeleteRangeAsync(int[] ids)
        {
            await base.DeleteRangeAsync(ids);
        }
    }
}