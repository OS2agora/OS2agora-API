using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;

namespace Agora.Operations.Common.Interfaces.DAOs
{
    public interface IUserDao
    {
        Task<User> GetAsync(int id, IncludeProperties includes = null);
        Task<List<User>> GetAllAsync(IncludeProperties includes = null);
        Task<User> UpdateAsync(User model, IncludeProperties includes = null);
        Task<User> CreateAsync(User model, IncludeProperties includes = null);
        Task<List<User>> CreateRangeAsync(List<User> models, IncludeProperties includes = null);
        Task<User> FindUserByIdentifier(string identifier, IncludeProperties includes = null);
        Task<User> FindUserByPersonalIdentifier(string personalIdentifier, IncludeProperties includes = null);
        Task<User> FindUserByEmail(string email, IncludeProperties includes = null);
    }
}