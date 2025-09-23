using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ICompanyHearingRoleDao
    {
        Task<CompanyHearingRole> GetAsync(int id, IncludeProperties includes = null);
        Task<List<CompanyHearingRole>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<CompanyHearingRole, bool>> filter = null);
        Task<CompanyHearingRole> CreateAsync(CompanyHearingRole model, IncludeProperties includes = null);
        Task<List<CompanyHearingRole>> CreateRangeAsync(List<CompanyHearingRole> models, IncludeProperties includes = null);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(int[] ids);
        Task<List<CompanyHearingRole>> GetCompanyHearingRolesForHearing(int hearingId,
            IncludeProperties includes = null);
    }
}
