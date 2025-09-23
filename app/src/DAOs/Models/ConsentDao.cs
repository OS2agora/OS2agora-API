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
    public class ConsentDao : BaseDao<ConsentEntity, Consent>, IConsentDao
    {
        public ConsentDao(IApplicationDbContext db, ILogger<BaseDao<ConsentEntity, Consent>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<Consent> CreateAsync(Consent model, IncludeProperties includes = null)
        {
            var consentEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(consentEntity, includes);
        }

        public async Task<Consent> UpdateAsync(Consent model, IncludeProperties includes = null)
        {
            var consentEntity = await base.UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(consentEntity, includes);
        }
    }
}