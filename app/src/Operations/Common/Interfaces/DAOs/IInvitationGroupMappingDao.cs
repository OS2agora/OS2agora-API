using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IInvitationGroupMappingDao
    {
        Task<List<InvitationGroupMapping>> CreateRangeAsync(List<InvitationGroupMapping> models, IncludeProperties includes = null);
        Task DeleteRangeAsync(int[] ids);
    }
}