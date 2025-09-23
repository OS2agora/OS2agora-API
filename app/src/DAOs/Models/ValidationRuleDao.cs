using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.DAOs.Statistics;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using System.Linq;
using System.Collections.Generic;


namespace BallerupKommune.DAOs.Models
{
    public class ValidationRuleDao : BaseDao<ValidationRuleEntity, ValidationRule>, IValidationRuleDao
    {
        public ValidationRuleDao(IApplicationDbContext db, ILogger<BaseDao<ValidationRuleEntity, ValidationRule>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics)
            : base(db, logger, mapper, commandCountStatistics) { }

        public new async Task<List<ValidationRule>> GetAllAsync(IncludeProperties includes = null)
        {
            var validationRuleEntities = await base.GetAllAsync(includes);
            return validationRuleEntities.Select(validationRuleEntity => MapAndPrune(validationRuleEntity, includes)).ToList();
        }
    }
}
