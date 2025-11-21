using Agora.Models.Common;
using Agora.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IInvitationSourceDao
    {
        Task<InvitationSource> GetAsync(int id, IncludeProperties includes = null);
        Task<List<InvitationSource>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<InvitationSource, bool>> filter = null);
    }
}