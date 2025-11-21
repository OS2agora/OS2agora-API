using Agora.Models.Common;
using Agora.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IInvitationGroupDao
    {
        Task<InvitationGroup> GetAsync(int id, IncludeProperties includes = null);
        Task<List<InvitationGroup>> GetAllAsync(IncludeProperties includes = null);
        Task<InvitationGroup> CreateAsync(InvitationGroup model, IncludeProperties includes = null);
        Task<InvitationGroup> UpdateAsync(InvitationGroup model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}