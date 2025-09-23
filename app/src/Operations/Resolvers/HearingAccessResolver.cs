using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BallerupKommune.Models.Common;
using NovaSec.Attributes;
using BallerupKommune.Operations.Common.Behaviours;
using MediatR;
using BallerupKommune.Operations.Models.Hearings.Queries.GetHearingsAfterSecurity;
using System.Net.Http;
using BallerupKommune.Operations.Common.Interfaces;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.Resolvers
{
    /// <summary>
    /// This resolver interface is in charge of finding all the hearing id's your allowed to see
    /// It caches the Hearingdata in its session (scoped)
    /// </summary>
    public interface IHearingAccessResolver
    {
        public Task<bool> CanSeeHearingById(int? hearingId = null);

        public Task<bool> CanSeeHearingBySubjectAreaId(int? SubjectAreaId);

        public Task<bool> CanHearingShowComments(int hearingId);

        public Task<List<Hearing>> GetHearingsAsync();
    }

    public class HearingAccessResolver : IHearingAccessResolver
    {
        private ISender _mediator;
        private List<Hearing> _cachedHearings = new List<Hearing>();

        public HearingAccessResolver(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Checks if data is cached if not then caches it
        /// </summary>
        private async Task EnsureLoaded()
        {
            if (!_cachedHearings.Any())
            {
                await LoadHearingsAfterSecurity();
            }
        }

        /// <summary>
        /// Sends a mediator call for Hearings since postfilters will only run through a mediator
        /// <param name="userId"> the userId for the logged in user</param>
        /// </summary>
        private async Task LoadHearingsAfterSecurity()
        {
            var operationResponse = await _mediator.Send(new GetHearingAccessIdsQuery());
            _cachedHearings = operationResponse;
        }

        /// <summary>
        /// Finds out if user is allowed to see hearing
        /// </summary>
        /// <param name="hearingId"> the HearingId that needs to be checked  user</param>
        /// <returns></returns>
        public async Task<bool> CanSeeHearingById(int? hearingId)
        {
            await EnsureLoaded();
            int? hearingAccessId = _cachedHearings.FirstOrDefault(hearing => hearing.Id == hearingId)?.Id;
            return hearingAccessId != null;
        }

        /// <summary>
        /// Finds out if subjectarea can be seen through hearings
        /// </summary>
        /// <param name="subjectAreaId"> the SubjectAreaId that needs to be checked  user</param>
        /// <returns></returns>
        public async Task<bool> CanSeeHearingBySubjectAreaId(int? subjectAreaId)
        {
            await EnsureLoaded();
            bool hearingAccessId = _cachedHearings.Any(hearing => hearing.SubjectAreaId == subjectAreaId);
            return hearingAccessId;
        }

        public async Task<bool> CanHearingShowComments(int hearingId)
        {
            await EnsureLoaded();
            Hearing hearing = _cachedHearings.Find(hearing => hearing.Id == hearingId);
            if (hearing != null)
            {
                return hearing.ShowComments;
            }
            return false;
        }

        /// <summary>
        /// Finds the hearing's the user is allowed to see
        /// </summary>
        /// <returns></returns>
        public async Task<List<Hearing>> GetHearingsAsync()
        {
            await EnsureLoaded();
            return _cachedHearings;
        }
    }
}
