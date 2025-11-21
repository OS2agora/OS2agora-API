using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
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