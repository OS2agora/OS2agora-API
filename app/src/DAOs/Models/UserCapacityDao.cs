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
    public class UserCapacityDao : BaseDao<UserCapacityEntity, UserCapacity>, IUserCapacityDao
    {
        public UserCapacityDao(IApplicationDbContext db, ILogger<BaseDao<UserCapacityEntity, UserCapacity>> logger,
            IMapper mapper, ICommandCountStatistics commandCountStatistics) : base(db, logger, mapper, commandCountStatistics)
        {
        }

        public async Task<List<UserCapacity>> GetAllAsync(IncludeProperties includes = null)
        {
            var userCapacityEntities = await base.GetAllAsync(includes);
            return userCapacityEntities.Select(userCapacityEntity => MapAndPrune(userCapacityEntity, includes)).ToList();
        }
    }
}