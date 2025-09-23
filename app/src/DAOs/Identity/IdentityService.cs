using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BallerupKommune.DAOs.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }

        public async Task<string> CreateUserAsync(string userName)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
            };

            await _userManager.CreateAsync(user);

            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<string> FindUserOrCreateUser(string personalIdentifier)
        {
            var possibleUser = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName == personalIdentifier);
            if (possibleUser == null)
            {
                var newlyCreatedUserId = await CreateUserAsync(personalIdentifier);
                return newlyCreatedUserId;
            }

            return possibleUser.Id;
        }

        public async Task AddUserToRole(string userId, string role)
        {
            var currentUser = await _userManager.Users.FirstAsync(u => u.Id == userId);
            var isInRole = await _userManager.IsInRoleAsync(currentUser, role);
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(currentUser, role);
            }
        }

        public async Task<bool> IsUserInRole(string userId, string role)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var isInRole = await _userManager.IsInRoleAsync(user, role);
            return isInRole;
        }
    }
}