using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<string> CreateUserAsync(string userName);
        Task<bool> DeleteUserAsync(string userId);
        Task<string> FindUserOrCreateUser(string userName);
        Task AddUserToRole(string userId, string role);
        Task<bool> IsUserInRole(string userId, string role);
    }
}