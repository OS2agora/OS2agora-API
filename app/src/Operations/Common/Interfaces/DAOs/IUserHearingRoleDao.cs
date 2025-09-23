using System.Collections.Generic;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IUserHearingRoleDao
    {
        Task<List<UserHearingRole>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<UserHearingRole, bool>> filter = null);
        Task<UserHearingRole> GetAsync(int id, IncludeProperties includes = null);
        Task<List<UserHearingRole>> GetUserHearingRolesForHearing(int hearingId, IncludeProperties includes = null);
        Task<UserHearingRole> CreateAsync(UserHearingRole model, IncludeProperties includes = null);
        Task<List<UserHearingRole>> CreateRangeAsync(List<UserHearingRole> models, IncludeProperties includes = null);
        Task<UserHearingRole> UpdateAsync(UserHearingRole model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(int[] ids);
    }
}