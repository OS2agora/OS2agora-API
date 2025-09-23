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
    public class HearingTemplateDao : BaseDao<HearingTemplateEntity, HearingTemplate>, IHearingTemplateDao
    {
        public HearingTemplateDao(IApplicationDbContext db, ILogger<BaseDao<HearingTemplateEntity, HearingTemplate>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics)
            : base(db, logger, mapper, commandCountStatistics) { }

        public async Task<List<HearingTemplate>> GetAllAsync(IncludeProperties includes = null)
        {
            var hearingTemplateEntities = await base.GetAllAsync(includes);
            return hearingTemplateEntities.Select(hearingTemplateEntity => MapAndPrune(hearingTemplateEntity, includes)).ToList();
        }
    }
}
