using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BallerupKommune.DAOs.Mappings;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace BallerupKommune.DAOs.Models
{
    public class ContentDao : BaseDao<ContentEntity, Content>, IContentDao
    {
        public ContentDao(IApplicationDbContext db, ILogger<BaseDao<ContentEntity, Content>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Content> GetAsync(int id, IncludeProperties includes = null)
        {
            var contentEntity = await base.GetAsync(id, includes);
            return MapAndPrune(contentEntity, includes);
        }

        public async Task<List<Content>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Content, bool>> filter = null)
        {
            List<Content> contentEntities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<Content, ContentEntity>());
            return contentEntities.Select(contentEntity => MapAndPrune(contentEntity, includes)).ToList();
        }

        public new async Task<Content> CreateAsync(Content model, IncludeProperties includes = null)
        {
            var contentEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(contentEntity, includes);
        }

        public async Task<Content> UpdateAsync(Content model, IncludeProperties includes = null)
        {
            var contentEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(contentEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}