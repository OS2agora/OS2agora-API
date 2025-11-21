using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace Agora.DAOs.Models
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