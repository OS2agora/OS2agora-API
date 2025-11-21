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
