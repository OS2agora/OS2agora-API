using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace BallerupKommune.DAOs.Models
{
    public class HearingRoleDao : BaseDao<HearingRoleEntity, HearingRole>, IHearingRoleDao
    {
        public HearingRoleDao(IApplicationDbContext db, ILogger<BaseDao<HearingRoleEntity, HearingRole>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<HearingRole> GetAsync(int id, IncludeProperties includes = null)
        {
            var hearingRoleEntity = await base.GetAsync(id, includes);
            return MapAndPrune(hearingRoleEntity, includes);
        }

        public new async Task<List<HearingRole>> GetAllAsync(IncludeProperties includes = null)
        {
            var hearingRoleEntities = await base.GetAllAsync(includes);
            return hearingRoleEntities.Select(hearingRoleEntity => MapAndPrune(hearingRoleEntity, includes)).ToList();
        }
    }
}