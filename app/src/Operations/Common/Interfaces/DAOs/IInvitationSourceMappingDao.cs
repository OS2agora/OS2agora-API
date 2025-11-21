using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IInvitationSourceMappingDao
    {
        Task<List<InvitationSourceMapping>> CreateRangeAsync(List<InvitationSourceMapping> models,
            IncludeProperties includes = null);
        Task<List<InvitationSourceMapping>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<InvitationSourceMapping, bool>> filter = null);
        Task DeleteRangeAsync(int[] ids);
    }
}