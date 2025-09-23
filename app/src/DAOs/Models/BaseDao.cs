using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.DAOs.Utility;
using BallerupKommune.Entities.Common;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public abstract class BaseDao<TEntity, TModel>
        where TEntity : BaseEntity
        where TModel : BaseModel
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICommandCountStatistics _commandCountStatistics;

        protected readonly ILogger<BaseDao<TEntity, TModel>> Logger;

        protected BaseDao(IApplicationDbContext db, ILogger<BaseDao<TEntity, TModel>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics)
        {
            _db = db;
            Logger = logger;
            _mapper = mapper;
            _commandCountStatistics = commandCountStatistics;
        }

        protected async Task<TModel> GetAsync(int id, IncludeProperties includes = null)
        {
            using Activity activity = StartDaoActivity(includes);
            try
            {
                includes?.ValidateRequestIncludes<TEntity>();
                var allIncludes = includes?.AllIncludes.ToList() ?? new List<string>();

                Logger.LogDebug($"GetAsync doing {typeof(TEntity).Name} call with {allIncludes.Count} includes: ({string.Join(',', allIncludes)})");
                var result = await _db.GetEntityAsync<TEntity>(id, allIncludes);
                Logger.LogDebug($"GetAsync did {typeof(TEntity).Name} call {_commandCountStatistics.CommandCount} EF commands executed.");
                string[] projections = allIncludes.ToArray();

                TModel model;
                using (Activity projectionActivity = StartProjectionActivity(projections))
                {
                    model = await _mapper.ProjectTo<TModel>(result, null, projections).SingleOrDefaultAsync();
                }

                return model;
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while fetching entity: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task<List<TModel>> GetAllAsync(IncludeProperties includes = null, Expression<Func<TEntity, bool>> filter = null)
        {
            using Activity activity = StartDaoActivity(includes, filter);
            try
            {
                includes?.ValidateRequestIncludes<TEntity>();
                var allIncludes = includes?.AllIncludes.ToList() ?? new List<string>();

                // debug message
                Logger.LogDebug($"GetAllAsync doing {typeof(TEntity).Name} call with {allIncludes.Count} includes: ({string.Join(',', allIncludes)})");
                var result = await _db.GetEntitiesAsync<TEntity>(includes: allIncludes, filter: filter);
                string[] projections = allIncludes.ToArray();

                List<TModel> models;
                using (Activity projectionActivity = StartProjectionActivity(projections))
                {
                    models = await _mapper.ProjectTo<TModel>(result, null, projections).ToListAsync();
                }

                Logger.LogDebug($"GetAllAsync did {typeof(TEntity).Name} call {_commandCountStatistics.CommandCount} EF commands executed.");
                return models;
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while fetching entities: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task<TModel> CreateAsync(TModel model, IncludeProperties includes = null)
        {
            using Activity activity = StartDaoActivity(includes);
            try
            {
                includes?.ValidateRequestIncludes<TEntity>();
                TEntity entity = ModelToEntity(model);
                await _db.CreateEntityAsync(entity);
                return await GetAsync(entity.Id, includes);
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while creating entity: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task<List<TModel>> CreateRangeAsync(List<TModel> models, IncludeProperties includes = null)
        {
            using Activity activity = StartDaoActivity(includes);
            try
            {
                includes?.ValidateRequestIncludes<TEntity>();
                List<TEntity> entities = models.Select(ModelToEntity).ToList();
                await _db.CreateEntitiesAsync(entities);
                return await GetAllAsync(includes);
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while creating entities: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task<TModel> UpdateAsync(TModel model, List<string> updatedProperties, IncludeProperties includes = null)
        {
            using Activity activity = StartDaoActivity(includes);
            try
            {
                includes?.ValidateRequestIncludes<TEntity>();
                TEntity entity = ModelToEntity(model);
                await _db.UpdateEntityAsync(entity, updatedProperties);
                return await GetAsync(entity.Id, includes);
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while updating entity: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task DeleteAsync(int id)
        {
            using Activity activity = StartDaoActivity();
            try
            {
                await _db.DeleteEntityAsync<TEntity>(id);
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while deleting entity: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected async Task DeleteRangeAsync(int[] ids)
        {
            using Activity activity = StartDaoActivity();
            try
            {
                await _db.DeleteEntitiesAsync<TEntity>(ids);
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred while deleting entities: {typeof(TEntity).Name}", e);
                throw;
            }
        }

        protected TModel MapAndPrune(TModel model, IncludeProperties includes)
        {
            return ModelPruneUtility.PruneIncludes(model, includes);
        }

        private TEntity ModelToEntity(TModel model)
        {
            return _mapper.Map<TEntity>(model);
        }

        private static Activity StartDaoActivity(IncludeProperties includes = null,
            Expression<Func<TEntity, bool>> filter = null, [CallerMemberName] string methodName = "")
        {
            Activity activity = Instrumentation.Source.StartActivity($"{methodName}: {typeof(TModel).Name}");
            activity?.SetTag("dao.includes", includes?.AllIncludes.ToArray() ?? Array.Empty<string>());
            activity?.SetTag("dao.filter", filter?.ToString());
            activity?.SetTag("dao.entity.type", typeof(TEntity).Name);
            return activity;
        }

        private static Activity StartProjectionActivity(string[] projections)
        {
            Activity activity = Instrumentation.Source.StartActivity("Automapper projection");
            activity?.SetTag("dao.automapper.projections", projections);
            return activity;
        }
    }
}

