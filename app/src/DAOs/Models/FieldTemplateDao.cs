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

namespace BallerupKommune.DAOs.Models
{
    public class FieldTemplateDao : BaseDao<FieldTemplateEntity, FieldTemplate>, IFieldTemplateDao
    {
        public FieldTemplateDao(IApplicationDbContext db, ILogger<BaseDao<FieldTemplateEntity, FieldTemplate>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<FieldTemplate>> GetAllAsync(IncludeProperties includes = null)
        {
            var fieldTemplateEntities = await base.GetAllAsync(includes);
            return fieldTemplateEntities.Select(fieldTemplateEntity => MapAndPrune(fieldTemplateEntity, includes)).ToList();
        }

        public new async Task<FieldTemplate> CreateAsync(FieldTemplate model, IncludeProperties includes = null)
        {
            var fieldTemplateEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(fieldTemplateEntity, includes);
        }

        public async Task<FieldTemplate> UpdateAsync(FieldTemplate model, IncludeProperties includes = null)
        {
            var fieldTemplateEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(fieldTemplateEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}