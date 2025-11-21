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

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}