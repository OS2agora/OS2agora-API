using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace Agora.DAOs.Models
{
    public class HearingStatusDao : BaseDao<HearingStatusEntity, HearingStatus>, IHearingStatusDao
    {
        public HearingStatusDao(IApplicationDbContext db, ILogger<BaseDao<HearingStatusEntity, HearingStatus>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<HearingStatus> GetAsync(int id, IncludeProperties includes = null)
        {
            var hearingStatusEntity = await base.GetAsync(id, includes);
            return MapAndPrune(hearingStatusEntity, includes);
        }

        public new async Task<List<HearingStatus>> GetAllAsync(IncludeProperties includes = null)
        {
            var hearingStatusEntities = await base.GetAllAsync(includes);
            return hearingStatusEntities.Select(hearingStatusEntity => MapAndPrune(hearingStatusEntity, includes)).ToList();
        }
    }
}