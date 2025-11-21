using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Agora.DAOs.Mappings;

namespace Agora.DAOs.Models
{
    public class InvitationSourceMappingDao : BaseDao<InvitationSourceMappingEntity, InvitationSourceMapping>, IInvitationSourceMappingDao
    {
        public InvitationSourceMappingDao(IApplicationDbContext db, ILogger<BaseDao<InvitationSourceMappingEntity, InvitationSourceMapping>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<InvitationSourceMapping>> CreateRangeAsync(List<InvitationSourceMapping> models, IncludeProperties includes)
        {
            var entities = await base.CreateRangeAsync(models, includes);
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public async Task<List<InvitationSourceMapping>> GetAllAsync(IncludeProperties includes = null, Expression<Func<InvitationSourceMapping, bool>> filter = null)
        {
            var entities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<InvitationSourceMapping, InvitationSourceMappingEntity>());
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }

        public new async Task DeleteRangeAsync(int[] ids)
        {
            await base.DeleteRangeAsync(ids);
        }
    }
}
