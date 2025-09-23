using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.Models.UserHearingRoles.Commands.CreateUserHearingRole
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId) && !@Security.IsHearingOwnerRole(#request.UserHearingRole.HearingRoleId)")]
    [PreAuthorize("HasRole('Administrator') && @Security.IsHearingOwnerRole(#request.UserHearingRole.HearingRoleId)")]
    public class CreateUserHearingRoleCommand : IRequest<UserHearingRole>
    {
        public UserHearingRole UserHearingRole { get; set; }
        public int HearingId { get; set; }

        public class CreateUserHearingRoleCommandHandler : IRequestHandler<CreateUserHearingRoleCommand, UserHearingRole>
        {
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly IHearingDao _hearingDao;
            private readonly IHearingRoleDao _hearingRoleDao;
            private readonly IPluginService _pluginService;

            public CreateUserHearingRoleCommandHandler(IUserHearingRoleDao userHearingRoleDao, IHearingDao hearingDao, IHearingRoleDao hearingRoleDao, IPluginService pluginService)
            {
                _userHearingRoleDao = userHearingRoleDao;
                _hearingDao = hearingDao;
                _hearingRoleDao = hearingRoleDao;
                _pluginService = pluginService;
            }

            public async Task<UserHearingRole> Handle(CreateUserHearingRoleCommand request, CancellationToken cancellationToken)
            {
                IncludeProperties hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(BallerupKommune.Models.Models.UserHearingRole.HearingRole)}"
                });
                var hearing = await _hearingDao.GetAsync(request.HearingId, hearingIncludes);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var hearingRoleFromRequest = await _hearingRoleDao.GetAsync(request.UserHearingRole.HearingRoleId);

                if (hearingRoleFromRequest == null)
                {
                    throw new NotFoundException(nameof(HearingRole), request.UserHearingRole.HearingRoleId);
                }

                request.UserHearingRole = await _pluginService.BeforeUserHearingRoleCreate(request.UserHearingRole);

                if (hearingRoleFromRequest.Role == HearingRole.HEARING_OWNER)
                {
                    var existingHearingOwner = hearing.UserHearingRoles.Single(x => x.HearingRole.Role == HearingRole.HEARING_OWNER);
                    await _userHearingRoleDao.DeleteAsync(existingHearingOwner.Id);
                }

                var includesForPlugin = IncludeProperties.Create<UserHearingRole>(null, new List<string> { nameof(BallerupKommune.Models.Models.UserHearingRole.Hearing), nameof(BallerupKommune.Models.Models.UserHearingRole.HearingRole) });
                var userHearingRole = await _userHearingRoleDao.CreateAsync(request.UserHearingRole, includesForPlugin);

                var defaultIncludes = IncludeProperties.Create<UserHearingRole>();
                var userHearingRoleWithPluginUpdates = await _pluginService.AfterUserHearingRoleCreate(userHearingRole);
                var result = await _userHearingRoleDao.UpdateAsync(userHearingRoleWithPluginUpdates, defaultIncludes);

                if (hearingRoleFromRequest.Role == HearingRole.HEARING_OWNER)
                {
                    await _pluginService.NotifyAfterChangeHearingOwner(hearing.Id);
                }

                return result;
            }
        }
    }
}