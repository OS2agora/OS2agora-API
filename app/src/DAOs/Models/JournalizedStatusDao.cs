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
    public class JournalizedStatusDao : BaseDao<JournalizedStatusEntity, JournalizedStatus>, IJournalizeStatusDao
    {
        public JournalizedStatusDao(IApplicationDbContext db, ILogger<BaseDao<JournalizedStatusEntity, JournalizedStatus>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<List<JournalizedStatus>> GetAllAsync(IncludeProperties includes = null)
        {
            var journalizedStatusEntities = await base.GetAllAsync(includes);
            return journalizedStatusEntities.Select(journalizedStatusEntity => MapAndPrune(journalizedStatusEntity, includes)).ToList();
        }
    }
}
