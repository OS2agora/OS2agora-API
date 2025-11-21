using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Agora.DAOs.Mappings;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace Agora.DAOs.Models
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