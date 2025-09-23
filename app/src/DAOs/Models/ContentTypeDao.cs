using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace BallerupKommune.DAOs.Models
{
    public class ContentTypeDao : BaseDao<ContentTypeEntity, ContentType>, IContentTypeDao
    {
        public ContentTypeDao(IApplicationDbContext db, ILogger<BaseDao<ContentTypeEntity, ContentType>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<ContentType>> GetAllAsync(IncludeProperties includes = null)
        {
            var contentTypeEntities = await base.GetAllAsync(includes);
            return contentTypeEntities.Select(contentTypeEntity => MapAndPrune(contentTypeEntity, includes)).ToList();
        }
    }
}