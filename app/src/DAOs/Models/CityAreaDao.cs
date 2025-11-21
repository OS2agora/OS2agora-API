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
    public class CityAreaDao : BaseDao<CityAreaEntity, CityArea>, ICityAreaDao
    {
        public CityAreaDao(IApplicationDbContext db, ILogger<BaseDao<CityAreaEntity, CityArea>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics)
            : base(db, logger, mapper, commandCountStatistics) { }

        public new async Task<CityArea> GetAsync(int id, IncludeProperties includes = null)
        {
            var cityAreaEntity = await base.GetAsync(id, includes);
            return MapAndPrune(cityAreaEntity, includes);
        }

        public new async Task<List<CityArea>> GetAllAsync(IncludeProperties includes = null)
        {
            var cityAreaEntities = await base.GetAllAsync(includes);
            return cityAreaEntities.Select(cityAreaEntity => MapAndPrune(cityAreaEntity, includes)).ToList();
        }

        public new async Task<CityArea> CreateAsync(CityArea model, IncludeProperties includes = null)
        {
            var cityAreaEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(cityAreaEntity, includes);
        }

        public async Task<CityArea> UpdateAsync(CityArea model, IncludeProperties includes = null)
        {
            var cityAreaEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(cityAreaEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}