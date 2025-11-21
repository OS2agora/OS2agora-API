using System.Collections.Generic;
using Agora.Models.Common;
using Agora.Models.Models;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface ICommentDao
    {
        Task<Comment> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Comment>> GetAllAsync(IncludeProperties includes = null, Expression<Func<Comment, bool>> filter = null);
        Task<Comment> CreateAsync(Comment model, IncludeProperties includes = null);
        Task<Comment> UpdateAsync(Comment model, IncludeProperties includes = null);
    }
}