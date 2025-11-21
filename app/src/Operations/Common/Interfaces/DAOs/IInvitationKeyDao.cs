using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IInvitationKeyDao
    {
        Task<List<InvitationKey>> CreateRangeAsync(List<InvitationKey> models, IncludeProperties includes = null);
        Task DeleteRangeAsync(int[] ids);
    }
}