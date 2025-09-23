using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class FieldDao : BaseDao<FieldEntity, Field>, IFieldDao
    {
        public FieldDao(IApplicationDbContext db, ILogger<BaseDao<FieldEntity, Field>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics) 
        {
        }

        public new async Task<Field> GetAsync(int id, IncludeProperties includes = null)
        {
            var fieldEntity = await base.GetAsync(id, includes);
            return MapAndPrune(fieldEntity, includes);
        }
    }
}