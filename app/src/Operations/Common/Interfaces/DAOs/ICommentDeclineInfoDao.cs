using Agora.Models.Common;
using Agora.Models.Models;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface ICommentDeclineInfoDao
    {
        Task<CommentDeclineInfo> GetAsync(int id, IncludeProperties includes = null);
        Task<CommentDeclineInfo> CreateAsync(CommentDeclineInfo model, IncludeProperties includes = null);
        Task<CommentDeclineInfo> UpdateAsync(CommentDeclineInfo model, IncludeProperties include = null);
        Task DeleteAsync(int id);
    }
}