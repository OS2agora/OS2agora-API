using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface ICommentDeclineInfoDao
    {
        Task<CommentDeclineInfo> GetAsync(int id, IncludeProperties includes = null);
        Task<CommentDeclineInfo> CreateAsync(CommentDeclineInfo model, IncludeProperties includes = null);
        Task<CommentDeclineInfo> UpdateAsync(CommentDeclineInfo model, IncludeProperties include = null);
        Task DeleteAsync(int id);
    }
}
