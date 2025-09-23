using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ICommentStatusDao
    {
        Task<List<CommentStatus>> GetAllAsync(IncludeProperties includes = null);
    }
}