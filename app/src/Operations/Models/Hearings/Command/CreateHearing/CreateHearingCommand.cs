using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using MediatR;
using NovaSec.Attributes;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Models.Hearings.Command.CreateHearing
{
    [PreAuthorize("HasRole('HearingCreator')")]
    public class CreateHearingCommand : IRequest<Hearing>
    {
        public List<string> RequestIncludes { get; set; }
        
        public class CreateHearingCommandHandler : IRequestHandler<CreateHearingCommand, Hearing>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly IJournalizeStatusDao _journalizedStatusDao;
            private readonly IUserDao _userDao;
            private readonly ICurrentUserService _currentUserService;
            private readonly IHearingRoleDao _hearingRoleDao;
            private readonly IIdentityService _identityService;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly IPluginService _pluginService;

            public CreateHearingCommandHandler(IHearingDao hearingDao, IHearingStatusDao hearingStatusDao,
                IJournalizeStatusDao journalizedStatusDao, IUserDao userDao,
                ICurrentUserService currentUserService, IHearingRoleDao hearingRoleDao,
                IIdentityService identityService, IUserHearingRoleDao userHearingRoleDao,
                IPluginService pluginService)
            {
                _hearingDao = hearingDao;
                _hearingStatusDao = hearingStatusDao;
                _journalizedStatusDao = journalizedStatusDao;
                _userDao = userDao;
                _currentUserService = currentUserService;
                _hearingRoleDao = hearingRoleDao;
                _identityService = identityService;
                _userHearingRoleDao = userHearingRoleDao;
                _pluginService = pluginService;
            }

            public async Task<Hearing> Handle(CreateHearingCommand request, CancellationToken cancellationToken)
            {
                // Creating a new Hearing requires the following steps:
                //  - Find the logged in user, in order to create the UserHearingRole
                //  - Find the DRAFT HearingStatus and find the HEARING_OWNER HearingRole
                //  - Then connect everything

                var userIdentifier = _currentUserService.UserId;

                if (userIdentifier == null)
                {
                    throw new Exception("Unauthorized access - No user was found");
                }

                var loggedInUser = await _userDao.FindUserByIdentifier(_currentUserService.UserId);

                if (loggedInUser == null)
                {
                    throw new NotFoundException(nameof(User), _currentUserService.UserId);
                }

                var canCreateHearings = await _identityService.IsUserInRole(userIdentifier, JWT.Roles.HearingCreator);

                if (!canCreateHearings)
                {
                    throw new Exception("User is not allowed to create hearings");
                }

                var hearingStatus = await _hearingStatusDao.GetAllAsync();
                var createdHearingStatus = hearingStatus.Single(status => status.Status == HearingStatus.CREATED);

                var journalizedStatuses = await _journalizedStatusDao.GetAllAsync();
                var notJournalizedStatus = journalizedStatuses.Single(status =>
                    status.Status == BallerupKommune.Models.Enums.JournalizedStatus.NOT_JOURNALIZED);

                var hearingRoles = await _hearingRoleDao.GetAllAsync();
                var hearingOwnerRole = hearingRoles.Single(hearingRole =>
                    hearingRole.Role == BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER);


                var newHearing = new Hearing
                {
                    HearingStatusId = createdHearingStatus.Id,
                    JournalizedStatusId = notJournalizedStatus.Id
                };

                newHearing = await _pluginService.BeforeHearingCreate(newHearing);

                var includes = IncludeProperties.Create<Hearing>(null, request.RequestIncludes);
                var hearing = await _hearingDao.CreateAsync(newHearing, includes);

                await _userHearingRoleDao.CreateAsync(new UserHearingRole
                {
                    HearingId = hearing.Id,
                    UserId = loggedInUser.Id,
                    HearingRoleId = hearingOwnerRole.Id
                });

                var hearingWithPluginUpdates = await _pluginService.AfterHearingCreate(hearing);
                var result = await _hearingDao.UpdateAsync(hearingWithPluginUpdates, includes);

                return result;
            }
        }
    }
}