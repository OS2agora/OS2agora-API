using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Agora.DAOs.Statistics;

namespace Agora.DAOs.Models
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