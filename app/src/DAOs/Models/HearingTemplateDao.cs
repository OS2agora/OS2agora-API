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
