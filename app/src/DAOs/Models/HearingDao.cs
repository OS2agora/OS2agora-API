using System;
using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Agora.DAOs.Mappings;

namespace Agora.DAOs.Models
{
    public class HearingDao : BaseDao<HearingEntity, Hearing>, IHearingDao
    {
        public HearingDao(IApplicationDbContext db, ILogger<BaseDao<HearingEntity, Hearing>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Hearing> GetAsync(int id, IncludeProperties includes = null, bool asNoTracking = false)
        {
            var hearingEntity = await base.GetAsync(id, includes, asNoTracking);
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