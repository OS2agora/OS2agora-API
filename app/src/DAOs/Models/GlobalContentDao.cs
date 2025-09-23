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
using GlobalContentType = BallerupKommune.Models.Enums.GlobalContentType;

namespace BallerupKommune.DAOs.Models
{
    public class GlobalContentDao : BaseDao<GlobalContentEntity, GlobalContent>, IGlobalContentDao
    {
        public GlobalContentDao(IApplicationDbContext db, ILogger<BaseDao<GlobalContentEntity, GlobalContent>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<GlobalContent>> GetAllAsync(IncludeProperties includes = null)
        {
            var globalContentEntities = await base.GetAllAsync(includes);
            return globalContentEntities.Select(globalContentEntity => MapAndPrune(globalContentEntity, includes)).ToList();
        }

        public async Task<GlobalContent> GetLatestVersionOfTypeAsync(GlobalContentType type, IncludeProperties includes = null)
        {
            if (includes == null)
            {
                includes = new IncludeProperties(null, new List<string> {nameof(GlobalContent.GlobalContentType)});
            }
            else if (!includes.IsSystemInclude(nameof(GlobalContent.GlobalContentType)))
            {
                includes.AddSystemInclude(nameof(GlobalContent.GlobalContentType));
            }
            var globalContents = await GetAllAsync(includes);
            var globalContentsOfType = globalContents.Where(x => x.GlobalContentType.Type == type).ToList();
            var latestVersion = globalContentsOfType.Max(x => x.Version);
            var globalContent = globalContents.SingleOrDefault(x => x.Version == latestVersion);
            return globalContent;
        }

        public new async Task<GlobalContent> CreateAsync(GlobalContent model, IncludeProperties includes = null)
        {
            var globalContentEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(globalContentEntity, includes);
        }
    }
}