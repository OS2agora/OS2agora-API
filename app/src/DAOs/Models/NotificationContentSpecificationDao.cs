using Agora.DAOs.Mappings;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.DAOs.Models;

public class NotificationContentSpecificationDao : BaseDao<NotificationContentSpecificationEntity, NotificationContentSpecification>, INotificationContentSpecificationDao
{
    public NotificationContentSpecificationDao(IApplicationDbContext db, ILogger<BaseDao<NotificationContentSpecificationEntity, NotificationContentSpecification>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
    {
    }

    public new async Task<NotificationContentSpecification> GetAsync(int id, IncludeProperties includes = null)
    {
        var entity = await base.GetAsync(id, includes);
        return MapAndPrune(entity, includes);
    }

    public async Task<List<NotificationContentSpecification>> GetAllAsync(IncludeProperties includes = null, Expression<Func<NotificationContentSpecification, bool>> filter = null)
    {
        var entities = await base.GetAllAsync(includes, filter?.MapToEntityExpression<NotificationContentSpecification, NotificationContentSpecificationEntity>());
        return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
    }

    public new async Task<NotificationContentSpecification> CreateAsync(NotificationContentSpecification model, IncludeProperties includes = null)
    {
        var entity = await base.CreateAsync(model, includes);
        return MapAndPrune(entity, includes);
    }

    public new async Task<List<NotificationContentSpecification>> CreateRangeAsync(List<NotificationContentSpecification> models, IncludeProperties includes = null)
    {
        var entities = await base.CreateRangeAsync(models, includes);
        return entities.Select(entity => MapAndPrune(entity, includes)).ToList();
    }

    public async Task<NotificationContentSpecification> UpdateAsync(NotificationContentSpecification model, IncludeProperties includes = null)
    {
        var entity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
        return MapAndPrune(entity, includes);
    }

    public new async Task DeleteAsync(int id)
    {
        await base.DeleteAsync(id);
    }
}