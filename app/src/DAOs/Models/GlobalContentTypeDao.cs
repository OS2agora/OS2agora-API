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
    public class GlobalContentTypeDao : BaseDao<GlobalContentTypeEntity, GlobalContentType>, IGlobalContentTypeDao
    {
        public GlobalContentTypeDao(IApplicationDbContext db, ILogger<BaseDao<GlobalContentTypeEntity, GlobalContentType>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<GlobalContentType>> GetAllAsync(IncludeProperties includes = null)
        {
            var globalContentTypeEntities = await base.GetAllAsync(includes);
            return globalContentTypeEntities.Select(globalContentTypeEntity => MapAndPrune(globalContentTypeEntity, includes)).ToList();
        }

        public new async Task<GlobalContentType> GetAsync(int id, IncludeProperties includes = null)
        {
            var globalContentTypeEntity = await base.GetAsync(id, includes);
            return MapAndPrune(globalContentTypeEntity, includes);
        }
    }
}