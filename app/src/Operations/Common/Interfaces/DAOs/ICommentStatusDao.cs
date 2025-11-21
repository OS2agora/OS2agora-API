using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface ICommentStatusDao
    {
        Task<List<CommentStatus>> GetAllAsync(IncludeProperties includes = null);
    }
}