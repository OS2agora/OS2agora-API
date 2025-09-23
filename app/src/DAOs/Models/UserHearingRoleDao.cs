using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Mappings;

namespace BallerupKommune.DAOs.Models
{
    public class UserHearingRoleDao: BaseDao<UserHearingRoleEntity, UserHearingRole>, IUserHearingRoleDao
    {
        public UserHearingRoleDao(IApplicationDbContext db, ILogger<BaseDao<UserHearingRoleEntity, UserHearingRole>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<UserHearingRole> GetAsync(int id, IncludeProperties includes = null)
        {
            var userHearingRoleEntity = await base.GetAsync(id, includes);
            return MapAndPrune(userHearingRoleEntity, includes);
        }

        public async Task<List<UserHearingRole>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<UserHearingRole, bool>> filter = null)
        {
            List<UserHearingRole> userHearingRoleEntities = await base.GetAllAsync(includes,
                filter?.MapToEntityExpression<UserHearingRole, UserHearingRoleEntity>());
            return userHearingRoleEntities.Select(userHearingRoleEntity => MapAndPrune(userHearingRoleEntity, includes))
                .ToList();
        }

        public new async Task<UserHearingRole> CreateAsync(UserHearingRole model, IncludeProperties includes = null)
        {
            var userHearingRoleEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(userHearingRoleEntity, includes);
        }

        public new async Task<List<UserHearingRole>> CreateRangeAsync(List<UserHearingRole> models,
            IncludeProperties includes = null)
        {
            var userHearingRoleEntities = await base.CreateRangeAsync(models, includes);
            return userHearingRoleEntities.Select(userHearingRoleEntity => MapAndPrune(userHearingRoleEntity, includes)).ToList();
        }

        public async Task<UserHearingRole> UpdateAsync(UserHearingRole model, IncludeProperties includes = null)
        {
            var userHearingRoleEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(userHearingRoleEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }

        public new async Task DeleteRangeAsync(int[] ids)
        {
            await base.DeleteRangeAsync(ids);
        }

        public new async Task<List<UserHearingRole>> GetUserHearingRolesForHearing(int hearingId, IncludeProperties includes = null)
        {
            return await GetAllAsync(includes, userHearingRole => userHearingRole.HearingId == hearingId);
        }
    }
}