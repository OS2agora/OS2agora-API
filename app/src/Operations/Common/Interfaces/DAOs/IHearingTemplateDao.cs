using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IHearingTemplateDao
    {
        Task<List<HearingTemplate>> GetAllAsync(IncludeProperties includes = null);
    }
}