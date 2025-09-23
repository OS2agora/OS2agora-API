using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IHearingTemplateDao
    {
        Task<List<HearingTemplate>> GetAllAsync(IncludeProperties includes = null);
    }
}