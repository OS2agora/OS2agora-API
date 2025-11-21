using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class KleHierarchyDao : BaseDao<KleHierarchyEntity, Agora.Models.Models.KleHierarchy>, IKleHierarchyDao
    {
        public KleHierarchyDao(IApplicationDbContext db, ILogger<BaseDao<KleHierarchyEntity, Agora.Models.Models.KleHierarchy>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Agora.Models.Models.KleHierarchy> GetAsync(int id, IncludeProperties includes = null)
        {
            var kleHierarchyEntity = await base.GetAsync(id, includes);
            return MapAndPrune(kleHierarchyEntity, includes);
        }

        public new async Task<List<Agora.Models.Models.KleHierarchy>> GetAllAsync(IncludeProperties includes = null)
        {
            var kleHierarchyEntities = await base.GetAllAsync(includes);
            return kleHierarchyEntities.Select(kleHierarchyEntity => MapAndPrune(kleHierarchyEntity, includes)).ToList();
        }
    }
}