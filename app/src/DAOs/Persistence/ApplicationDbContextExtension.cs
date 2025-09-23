using BallerupKommune.DAOs.Mappings;
using BallerupKommune.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence
{
    internal static class ApplicationDbContextExtension
    {
        private static DbContext DbAsDbContext(IApplicationDbContext db) => db as DbContext;

        internal static async Task<IQueryable<TEntity>> GetEntityAsync<TEntity>(
            this IApplicationDbContext db,
            int id,
            List<string> includes = null) where TEntity : BaseEntity
        {
            var query = await db.BuildQueryable<TEntity>(entity => entity.Id == id, includes);
            return query;
        }

        public static IQueryable<object> GetDbSet(this IApplicationDbContext context, Type entityType)
        {
            return (IQueryable<object>)context.GetType().GetMethod("Set").MakeGenericMethod(entityType).Invoke(context, null);
        }

        internal static async Task<IQueryable<TEntity>> GetEntitiesAsync<TEntity>(
            this IApplicationDbContext db,
            Expression<Func<TEntity, bool>> filter = null,
            List<string> includes = null
        ) where TEntity : BaseEntity
        {
            var query = await db.BuildQueryable(filter, includes);
            //var result = await query.ToListAsync();
            return query;
        }

        internal static async Task<IQueryable<TEntity>> BuildQueryable<TEntity>(
            this IApplicationDbContext db,
            Expression<Func<TEntity, bool>> filter = null,
            List<string> includes = null) where TEntity : BaseEntity
        {
            var query = DbAsDbContext(db).Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null && includes.Count > 0)
            {
                foreach (var navigationPath in includes)
                {
                    query = query.Include(navigationPath);
                }
            }

            return query.AsSplitQuery();
        }

        internal static async Task CreateEntityAsync<TEntity>(this IApplicationDbContext db, TEntity entity)
            where TEntity : BaseEntity
        {
            entity.FixRelationships();
            var query = DbAsDbContext(db).Set<TEntity>();

            query.Update(entity);
            await DbAsDbContext(db).SaveChangesAsync();
        }

        internal static async Task CreateEntitiesAsync<TEntity>(this IApplicationDbContext db,
            List<TEntity> entities) where TEntity : BaseEntity
        {
            foreach (var entity in entities)
            {
                entity.FixRelationships();
            }

            var query = DbAsDbContext(db).Set<TEntity>();
            query.UpdateRange(entities);
            await DbAsDbContext(db).SaveChangesAsync();
        }

        internal static async Task DeleteEntityAsync<TEntity>(this IApplicationDbContext db, int id) where TEntity : BaseEntity
        {
            TEntity entityToDelete = (await db.GetEntityAsync<TEntity>(id)).SingleOrDefault();

            DbAsDbContext(db).Remove(entityToDelete);
            await DbAsDbContext(db).SaveChangesAsync();
        }

        internal static async Task DeleteEntitiesAsync<TEntity>(this IApplicationDbContext db, int[] ids)
            where TEntity : BaseEntity
        {
            var allEntities = await db.GetEntitiesAsync<TEntity>();
            var entitiesToDelete = allEntities.Where(entity => ids.Any(id => entity.Id == id));

            DbAsDbContext(db).RemoveRange(entitiesToDelete);
            await DbAsDbContext(db).SaveChangesAsync();
        }

        internal static async Task UpdateEntityAsync<TEntity>(this IApplicationDbContext db, TEntity entity, List<string> updatedProperties)
            where TEntity : BaseEntity
        {
            entity.FixRelationships();

            TEntity currentEntity = (await db.GetEntityAsync<TEntity>(entity.Id)).Single();
            currentEntity.CopyUpdatedPropertiesAndRelationships(entity, updatedProperties);

            DbAsDbContext(db).Update(currentEntity);
            await DbAsDbContext(db).SaveChangesAsync();
        }
    }
}
