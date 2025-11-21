using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Agora.DAOs.Mappings;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace Agora.DAOs.Models
{
    public class CompanyHearingRoleDao : BaseDao<CompanyHearingRoleEntity, CompanyHearingRole>, ICompanyHearingRoleDao
    {
        public CompanyHearingRoleDao(IApplicationDbContext db, ILogger<BaseDao<CompanyHearingRoleEntity, CompanyHearingRole>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        { }

        public new async Task<CompanyHearingRole> GetAsync(int id, IncludeProperties includes = null)
        {
            var companyHearingRoleEntity = await base.GetAsync(id, includes);
            return MapAndPrune(companyHearingRoleEntity, includes);
        }

        public async Task<List<CompanyHearingRole>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<CompanyHearingRole, bool>> filter = null)
        {
            List<CompanyHearingRole> companyHearingRoleEntities = await base.GetAllAsync(includes,
                filter?.MapToEntityExpression<CompanyHearingRole, CompanyHearingRoleEntity>());
            return companyHearingRoleEntities.Select(companyHearingRoleEntity => MapAndPrune(companyHearingRoleEntity, includes))
                .ToList();
        }

        public new async Task<CompanyHearingRole> CreateAsync(CompanyHearingRole model, IncludeProperties includes = null)
        {
            var companyHearingRoleEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(companyHearingRoleEntity, includes);
        }

        public new async Task<List<CompanyHearingRole>> CreateRangeAsync(List<CompanyHearingRole> models,
            IncludeProperties includes = null)
        {
            var companyHearingRoleEntities = await base.CreateRangeAsync(models, includes);
            return companyHearingRoleEntities.Select(companyHearingRoleEntity => MapAndPrune(companyHearingRoleEntity, includes)).ToList();
        }

        public async Task<CompanyHearingRole> UpdateAsync(CompanyHearingRole model, IncludeProperties includes = null)
        {
            var companyHearingRoleEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(companyHearingRoleEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }

        public new async Task DeleteRangeAsync(int[] ids)
        {
            await base.DeleteRangeAsync(ids);
        }

        public new async Task<List<CompanyHearingRole>> GetCompanyHearingRolesForHearing(int hearingId, IncludeProperties includes = null)
        {
            return await GetAllAsync(includes, companyHearingRole => companyHearingRole.HearingId == hearingId);
        }
    }
}