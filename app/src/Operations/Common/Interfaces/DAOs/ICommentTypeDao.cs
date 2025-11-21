using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface ICommentTypeDao
    {
        Task<List<CommentType>> GetAllAsync(IncludeProperties includes = null);
    }
}