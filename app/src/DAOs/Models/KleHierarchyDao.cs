using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class KleHierarchyDao : BaseDao<KleHierarchyEntity, BallerupKommune.Models.Models.KleHierarchy>, IKleHierarchyDao
    {
        public KleHierarchyDao(IApplicationDbContext db, ILogger<BaseDao<KleHierarchyEntity, BallerupKommune.Models.Models.KleHierarchy>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<BallerupKommune.Models.Models.KleHierarchy> GetAsync(int id, IncludeProperties includes = null)
        {
            var kleHierarchyEntity = await base.GetAsync(id, includes);
            return MapAndPrune(kleHierarchyEntity, includes);
        }

        public new async Task<List<BallerupKommune.Models.Models.KleHierarchy>> GetAllAsync(IncludeProperties includes = null)
        {
            var kleHierarchyEntities = await base.GetAllAsync(includes);
            return kleHierarchyEntities.Select(kleHierarchyEntity => MapAndPrune(kleHierarchyEntity, includes)).ToList();
        }
    }
}