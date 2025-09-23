using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Statistics;

namespace BallerupKommune.DAOs.Models
{
    public class KleMappingDao : BaseDao<KleMappingEntity, KleMapping>, IKleMappingDao
    {
        public KleMappingDao(IApplicationDbContext db, ILogger<BaseDao<KleMappingEntity, KleMapping>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<KleMapping> CreateAsync(KleMapping model, IncludeProperties includes = null)
        {
            var kleMappingEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(kleMappingEntity, includes);
        }

        public new async Task<List<KleMapping>> CreateRangeAsync(List<KleMapping> models, IncludeProperties includes = null)
        {
            var kleMappingEntities = await base.CreateRangeAsync(models, includes);
            return kleMappingEntities.Select(kleMappingEntity => MapAndPrune(kleMappingEntity, includes)).ToList();
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }

        public new async Task DeleteRangeAsync(int[] ids)
        {
            await base.DeleteRangeAsync(ids);
        }
    }
}