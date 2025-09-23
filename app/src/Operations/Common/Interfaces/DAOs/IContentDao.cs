using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces.DAOs
{
    public interface IContentDao
    {
        Task<Content> GetAsync(int id, IncludeProperties includes = null);
        Task<List<Content>> GetAllAsync(IncludeProperties includes = null,
            Expression<Func<Content, bool>> filter = null);
        Task<Content> CreateAsync(Content model, IncludeProperties includes = null);
        Task<Content> UpdateAsync(Content model, IncludeProperties includes = null);
        Task DeleteAsync(int id);
    }
}