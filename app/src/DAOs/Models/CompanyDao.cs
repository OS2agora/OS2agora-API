using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;

namespace Agora.DAOs.Models
{
    public class CompanyDao : BaseDao<CompanyEntity, Company>, ICompanyDao
    {

        public CompanyDao(IApplicationDbContext db, ILogger<BaseDao<CompanyEntity, Company>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        { }

        public new async Task<Company> GetAsync(int id, IncludeProperties includes = null)
        {
            var companyEntity = await base.GetAsync(id, includes);
            return MapAndPrune(companyEntity, includes);
        }

        public new async Task<List<Company>> GetAllAsync(IncludeProperties includes = null)
        {
            List<Company> companyEntities =
                await base.GetAllAsync(includes);
            return companyEntities.Select(companyEntity => MapAndPrune(companyEntity, includes)).ToList();
        }

        public new async Task<Company> GetCompanyByCvr(string cvr, IncludeProperties includes = null)
        {
            // Filter must be applied in memory because Cvr column can be encrypted!
            var possibleCompanies = await GetAllAsync(includes);
            return possibleCompanies.SingleOrDefault(company => company.Cvr == cvr);
        }

        public new async Task<Company> CreateAsync(Company model, IncludeProperties includes = null)
        {
            var companyEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(companyEntity, includes);
        }

        public new async Task<List<Company>> CreateRangeAsync(List<Company> models, IncludeProperties includes = null)
        {
            var companyEntities = await base.CreateRangeAsync(models, includes);
            return companyEntities.Select(companyEntity => MapAndPrune(companyEntity, includes)).ToList();
        }

        public async Task<Company> UpdateAsync(Company model, IncludeProperties includes = null)
        {
            var companyEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(companyEntity, includes);
        }
    }
}