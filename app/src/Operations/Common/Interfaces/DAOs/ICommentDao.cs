using System.Collections.Generic;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ICommentDao
    {
        Task<Comment> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Comment>> GetAllAsync(IncludeProperties includes = null, List<int> hearingIds = null);
        Task<Comment> CreateAsync(Comment model, IncludeProperties includes = null);
        Task<Comment> UpdateAsync(Comment model, IncludeProperties includes = null);
    }
}