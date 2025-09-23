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
    public class SubjectAreaDao : BaseDao<SubjectAreaEntity, SubjectArea>, ISubjectAreaDao
    {
        public SubjectAreaDao(IApplicationDbContext db, ILogger<BaseDao<SubjectAreaEntity, SubjectArea>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) 
            : base(db, logger, mapper, commandCountStatistics) { }

        public new async Task<SubjectArea> GetAsync(int id, IncludeProperties includes = null)
        {
            var subjectAreaEntity = await base.GetAsync(id, includes);
            return MapAndPrune(subjectAreaEntity, includes);
        }

        public new async Task<List<SubjectArea>> GetAllAsync(IncludeProperties includes = null)
        {
            var subjectAreaEntities = await base.GetAllAsync(includes);
            return subjectAreaEntities.Select(subjectAreaEntity => MapAndPrune(subjectAreaEntity, includes)).ToList();
        }

        public new async Task<SubjectArea> CreateAsync(SubjectArea model, IncludeProperties includes = null)
        {
            var subjectAreaEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(subjectAreaEntity, includes);
        }

        public async Task<SubjectArea> UpdateAsync(SubjectArea model, IncludeProperties includes = null)
        {
            var subjectAreaEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(subjectAreaEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}
