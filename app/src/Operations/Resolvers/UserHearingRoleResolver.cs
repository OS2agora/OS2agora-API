using Agora.Operations.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;


namespace Agora.Operations.Resolvers
{
    public interface IUserHearingRoleResolver
    {
        /// <summary>
        /// Find if the user has certain role on specific hearing or in general
        /// </summary>
        /// <param name="hearingId"> if given, the resolver will check the specified hearing.
        /// Otherwise, it will check if the user has the role on any hearing.</param>
        /// <param name="role"> role that needs to be checked</param>
        /// <param name="userId"> if given the resolver will find the id's userhearingroles, else it will use logged in </param>
        /// <returns></returns>
        public Task<bool> UserHearingRoleExists(int? hearingId, HearingRoleEnum role, int? userId = null);

        /// <summary>
        /// Find out if the user is hearingowner on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the user is hearingOwner on specific hearing.
        /// Otherwise, it will check if the user has the role on any hearing.</param>
        /// <param name="userId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingOwner(int? hearingId = null, int? userId = null);

        /// <summary>
        /// Find out if the user is hearingreviewer on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the user is hearingReviewer on specific hearing.
        /// Otherwise, it will check if the user has the role on any hearing.</param>
        /// <param name="userId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingReviewer(int? hearingId = null, int? userId = null);

        /// <summary>
        /// Find out if the user is hearingresponder on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the user is hearingResponder on specific hearing.
        /// Otherwise, it will check if the user has the role on any hearing.</param>
        /// <param name="userId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingResponder(int? hearingId = null, int? userId = null);

        /// <summary>
        /// Find out if the user is hearinginvitee on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the user is hearingInvitee on specific hearing.
        /// Otherwise, it will check if the user has the role on any hearing.</param>
        /// <param name="userId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingInvitee(int? hearingId = null, int? userId = null);
    }

    public class UserHearingRoleResolver : IUserHearingRoleResolver
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserHearingRoleDao _userHearingRoleDao;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private Dictionary<int,List<UserHearingRole>> _cachedUserHearingsRoles = new Dictionary<int, List<UserHearingRole>>();

        public UserHearingRoleResolver( ICurrentUserService currentUserService, IUserHearingRoleDao userHearingRoleDao, IHearingRoleResolver hearingRoleResolver)
        {
            _currentUserService = currentUserService;
            _userHearingRoleDao = userHearingRoleDao;
            _hearingRoleResolver = hearingRoleResolver;
        }

        /// <inheritdoc/>
        /// note: The reason for nullable int on userid is if another users userhearingroles is needed 
        public async Task<bool> UserHearingRoleExists(int? hearingId, HearingRoleEnum role, int? userId = null)
        {
            userId ??= _currentUserService.DatabaseUserId;
            if (userId == null)
            {
                return false;
            }

            await EnsureLoaded(userId.Value);
            HearingRole hearingRole = await _hearingRoleResolver.GetHearingRole(role);

            if (hearingId == null)
            {
                return _cachedUserHearingsRoles[userId.Value].Any(
                    userHearingRole => userHearingRole.HearingRoleId == hearingRole.Id);
            }

            return _cachedUserHearingsRoles[userId.Value].Any(
                userHearingRole => userHearingRole.HearingId == hearingId
                                    && userHearingRole.HearingRoleId == hearingRole.Id);
        }

        /// <inheritdoc/>
        public async Task<bool> IsHearingOwner(int? hearingId, int? userId = null)
        {
            return await UserHearingRoleExists(hearingId, HearingRoleEnum.HEARING_OWNER, userId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsHearingReviewer(int? hearingId, int? userId = null)
        {
            return await UserHearingRoleExists(hearingId, HearingRoleEnum.HEARING_REVIEWER, userId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsHearingResponder(int? hearingId, int? userId = null)
        {
            return await UserHearingRoleExists(hearingId, HearingRoleEnum.HEARING_RESPONDER, userId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsHearingInvitee(int? hearingId, int? userId = null)
        {
            return await UserHearingRoleExists(hearingId, HearingRoleEnum.HEARING_INVITEE, userId);
        }

        /// <summary>
        /// Checks if data is cached if not then caches it
        /// </summary>
        private async Task EnsureLoaded( int userId)
        {
            if (!_cachedUserHearingsRoles.ContainsKey(userId))
            {
                await LoadUserHearingsRoles(userId);
            }

        }

        private async Task LoadUserHearingsRoles(int userId)
        {
            List<UserHearingRole> userHearingsRoles =
                await _userHearingRoleDao.GetAllAsync(null, userHearingRole => userHearingRole.UserId == userId);
            _cachedUserHearingsRoles[userId] = userHearingsRoles;
        }
    }
}
