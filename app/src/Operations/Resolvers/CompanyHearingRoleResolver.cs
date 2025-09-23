using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingRoleEnum = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.Resolvers
{
    public interface ICompanyHearingRoleResolver
    {
        /// <summary>
        /// Find if the company has certain role on specific hearing or in general
        /// </summary>
        /// <param name="hearingId"> if given, the resolver will check the specified hearing.
        /// Otherwise, it will check if the company has the role on any hearing.</param>
        /// <param name="role"> role that needs to be checked</param>
        /// <param name="companyId"> if given the resolver will find the id's companyhearingroles, else it will use logged in </param>
        /// <returns></returns>
        public Task<bool> CompanyHearingRoleExist(int? hearingId, HearingRoleEnum role, int? companyId = null);

        /// <summary>
        /// Find out if the company is hearingresponder on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the company is hearingResponder on specific hearing.
        /// Otherwise, it will check if the company has the role on any hearing.</param>
        /// <param name="companyId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingResponder(int? hearingId = null, int? companyId = null);

        /// <summary>
        /// Find out if the company is hearinginvitee on a specific or any hearing
        /// </summary>
        /// <param name="hearingId"> if given, check if the company is hearingInvitee on specific hearing.
        /// Otherwise, it will check if the company has the role on any hearing.</param>
        /// <param name="companyId"> userId to check comment's role on</param>
        /// <returns></returns>
        public Task<bool> IsHearingInvitee(int? hearingId = null, int? companyId = null);
    }
    public class CompanyHearingRoleResolver : ICompanyHearingRoleResolver
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private Dictionary<int, List<CompanyHearingRole>> _cachedCompanyHearingsRoles = new Dictionary<int, List<CompanyHearingRole>>();

        public CompanyHearingRoleResolver(ICurrentUserService currentUserService,
            ICompanyHearingRoleDao companyHearingRoleDao, IHearingRoleResolver hearingRoleResolver)
        {
            _currentUserService = currentUserService;
            _companyHearingRoleDao = companyHearingRoleDao;
            _hearingRoleResolver = hearingRoleResolver;
        }
        /// <inheritdoc/>
        public async Task<bool> CompanyHearingRoleExist(int? hearingId, HearingRoleEnum role, int? companyId = null)
        {
            companyId ??= _currentUserService.CompanyId;

            if (companyId == null)
            {
                return false;
            }

            await EnsureLoaded(companyId.Value);
            HearingRole hearingRole = await _hearingRoleResolver.GetHearingRole(role);

            if (hearingId == null)
            {
                return _cachedCompanyHearingsRoles[companyId.Value].Any(
                    chr => chr.HearingRoleId == hearingRole.Id);
            }

            return _cachedCompanyHearingsRoles[companyId.Value].Any(
                chr => chr.HearingId == hearingId 
                       && chr.HearingRoleId == hearingRole.Id);

        }
        /// <inheritdoc/>
        public async Task<bool> IsHearingResponder(int? hearingId = null, int? companyId = null)
        {
            return await CompanyHearingRoleExist(hearingId, HearingRoleEnum.HEARING_RESPONDER, companyId);
        }
        /// <inheritdoc/>
        public async Task<bool> IsHearingInvitee(int? hearingId = null, int? companyId = null)
        {
            return await CompanyHearingRoleExist(hearingId, HearingRoleEnum.HEARING_INVITEE, companyId);
        }

        /// <summary>
        /// Checks if data is cached if not then caches it
        /// </summary>
        private async Task EnsureLoaded(int companyId)
        {
            if (!_cachedCompanyHearingsRoles.ContainsKey(companyId))
            {
                await LoadCompanyHearingRoles(companyId);
            }
        }

        private async Task LoadCompanyHearingRoles(int companyId)
        {
            List<CompanyHearingRole> companyHearingRoles =
                await _companyHearingRoleDao.GetAllAsync(null, chr => chr.CompanyId == companyId);

            _cachedCompanyHearingsRoles[companyId] = companyHearingRoles;
        }
    }
}
