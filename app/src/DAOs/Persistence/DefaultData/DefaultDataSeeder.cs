using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence.DefaultData
{
    public abstract class DefaultDataSeeder<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly List<TEntity> _defaultEntities;
        protected readonly Func<TEntity, TEntity, bool> _comparer;

        public DefaultDataSeeder(ApplicationDbContext context, DbSet<TEntity> dbSet, List<TEntity> defaultEntities, Func<TEntity, TEntity, bool> comparer)
        {
            _context = context;
            _dbSet = dbSet;
            _defaultEntities = defaultEntities;
            _comparer = comparer;
        }

        public async Task SeedEntitiesAsync()
        {
            var existingEntities = await _dbSet.ToListAsync();

            var appendEntities = GetEntitiesToAppend(existingEntities);

            if (appendEntities.Any())
            {
                await _dbSet.AddRangeAsync(appendEntities);
                await _context.SaveChangesAsync();
            }

            var entitiesToUpdate = FetchEntitiesToUpdate(existingEntities, _defaultEntities);
            if (entitiesToUpdate.Any() && existingEntities.Any())
            {
                _dbSet.UpdateRange(entitiesToUpdate);
                await _context.SaveChangesAsync();
            }
        }

        private List<TEntity> GetEntitiesToAppend(List<TEntity> existingEntities)
        {
            return _defaultEntities
                .Where(entity => !existingEntities.Any(e => _comparer(e, entity)))
                .ToList();
        }

        public abstract List<TEntity> FetchEntitiesToUpdate(List<TEntity> existingEntities, List<TEntity> defaultEntities);
    }
}
