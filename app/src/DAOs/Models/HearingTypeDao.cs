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
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class HearingTypeDao : BaseDao<HearingTypeEntity, HearingType>, IHearingTypeDao
    {
        public HearingTypeDao(IApplicationDbContext db, ILogger<BaseDao<HearingTypeEntity, HearingType>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<HearingType> GetAsync(int id, IncludeProperties includes = null)
        {
            var hearingTypeEntity = await base.GetAsync(id, includes);
            return MapAndPrune(hearingTypeEntity, includes);
        }

        public new async Task<List<HearingType>> GetAllAsync(IncludeProperties includes = null)
        {
            var hearingTypeEntities = await base.GetAllAsync(includes);
            return hearingTypeEntities.Select(hearingTypeEntity => MapAndPrune(hearingTypeEntity, includes)).ToList();
        }

        public new async Task<HearingType> CreateAsync(HearingType model, IncludeProperties includes = null)
        {
            var hearingTypeEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(hearingTypeEntity, includes);
        }

        public async Task<HearingType> UpdateAsync(HearingType model, IncludeProperties includes = null)
        {
            var hearingTypeEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(hearingTypeEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}