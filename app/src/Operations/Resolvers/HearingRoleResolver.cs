using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.Resolvers
{
    /// <summary>
    /// Resolver for getting <see cref="HearingRole"/>s.
    /// </summary>
    public interface IHearingRoleResolver
    {
        /// <summary>
        /// Gets hearing role.
        /// </summary>
        /// <param name="hearingRoleEnum">The hearing role to get.</param>
        /// <returns>The requested hearing role. Will be <c>null</c> if it can not be found.</returns>
        Task<HearingRole> GetHearingRole(HearingRoleEnum hearingRoleEnum);

        /// <summary>
        /// Gets specified hearing roles.
        /// </summary>
        /// <param name="hearingRoleEnums">The hearing roles to get.</param>
        /// <returns>
        /// List of specified hearing roles. Hearing roles not able to be retrieved are excluded.
        /// </returns>
        Task<List<HearingRole>> GetHearingRoles(params HearingRoleEnum[] hearingRoleEnums);
    }

    /// <inheritdoc />
    public class HearingRoleResolver : IHearingRoleResolver
    {
        private readonly IHearingRoleDao _hearingRoleDao;
        private readonly ILogger<HearingRoleResolver> _logger;

        private Dictionary<HearingRoleEnum, HearingRole> _hearingRoleDictionary =
            new Dictionary<HearingRoleEnum, HearingRole>();

        public HearingRoleResolver(IHearingRoleDao hearingRoleDao, ILogger<HearingRoleResolver> logger)
        {
            _hearingRoleDao = hearingRoleDao;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<HearingRole> GetHearingRole(HearingRoleEnum hearingRoleEnum)
        {
            return (await GetHearingRoles(hearingRoleEnum)).SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<List<HearingRole>> GetHearingRoles(params HearingRoleEnum[] hearingRoleEnums)
        {
            await EnsureLoaded();
            return hearingRoleEnums.Select(GetHearingRoleFromDictionary).ToList();
        }

        private HearingRole GetHearingRoleFromDictionary(HearingRoleEnum hearingRoleEnum)
        {
            if (!_hearingRoleDictionary.TryGetValue(hearingRoleEnum, out HearingRole hearingRole))
            {
                _logger.LogWarning("Failed to get {HearingRole} hearing role.", hearingRoleEnum);
            }
            return hearingRole;
        }

        private async Task EnsureLoaded()
        {
            if (!_hearingRoleDictionary.Any())
            {
                await LoadHearingRoles();
            }
        }

        private async Task LoadHearingRoles()
        {
            List<HearingRole> hearingRoles = await _hearingRoleDao.GetAllAsync();
            _hearingRoleDictionary = hearingRoles.ToDictionary(role => role.Role);
        }
    }
}