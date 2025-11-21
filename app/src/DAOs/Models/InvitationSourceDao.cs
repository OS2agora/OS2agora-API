using Agora.DAOs.Mappings;
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

namespace Agora.DAOs.Models
{
    public class InvitationSourceDao : BaseDao<InvitationSourceEntity, InvitationSource>, IInvitationSourceDao
    {
        public InvitationSourceDao(IApplicationDbContext db, ILogger<BaseDao<InvitationSourceEntity, InvitationSource>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<InvitationSource> GetAsync(int id, IncludeProperties includes = null)
        {
            var entity = await base.GetAsync(id, includes);
            return MapAndPrune(entity, includes);
        }

        public async Task<List<InvitationSource>> GetAllAsync(IncludeProperties includes = null, Expression<Func<InvitationSource, bool>> filter = null)
        {
            var entities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<InvitationSource, InvitationSourceEntity>());
            return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
        }
    }
}
