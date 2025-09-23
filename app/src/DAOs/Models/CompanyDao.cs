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
    public class CompanyDao : BaseDao<CompanyEntity, Company>, ICompanyDao
    {
        public CompanyDao(IApplicationDbContext db, ILogger<BaseDao<CompanyEntity, Company>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics) { }

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
            var allCompanies = await GetAllAsync(includes);
            var possibleCompany = allCompanies.SingleOrDefault(company => company.Cvr == cvr);
            return possibleCompany;
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

    }
}
