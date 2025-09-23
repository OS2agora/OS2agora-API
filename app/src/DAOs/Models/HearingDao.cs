using System;
using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Mappings;

namespace BallerupKommune.DAOs.Models
{
    public class HearingDao : BaseDao<HearingEntity, Hearing>, IHearingDao
    {
        public HearingDao(IApplicationDbContext db, ILogger<BaseDao<HearingEntity, Hearing>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Hearing> GetAsync(int id, IncludeProperties includes = null)
        {
            var hearingEntity = await base.GetAsync(id, includes);
            return MapAndPrune(hearingEntity, includes);
        }

        public async Task<List<Hearing>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Hearing, bool>> filter = null)
        {
            List<Hearing> hearingEntities =
                await base.GetAllAsync(includes, filter?.MapToEntityExpression<Hearing, HearingEntity>());
            return hearingEntities.Select(hearingEntity => MapAndPrune(hearingEntity, includes)).ToList();
        }

        public new async Task<Hearing> CreateAsync(Hearing model, IncludeProperties includes = null)
        {
            var hearingEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(hearingEntity, includes);
        }

        public async Task<Hearing> UpdateAsync(Hearing model, IncludeProperties includes = null)
        {
            var hearingEntity = await UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(hearingEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}