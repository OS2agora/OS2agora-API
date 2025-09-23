using BallerupKommune.Models.Extensions;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Exceptions;
using NovaSec.Attributes;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.Models.UserHearingRoles.Commands.DeleteUserHearingRole
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class DeleteUserHearingRoleCommand : IRequest
    {
        public int Id { get; set; }
        public int HearingId { get; set; }

        public class DeleteUserHearingRoleCommandHandler : IRequestHandler<DeleteUserHearingRoleCommand>
        {
            private readonly IUserHearingRoleDao _userHearingRoleDao;

            public DeleteUserHearingRoleCommandHandler(IUserHearingRoleDao userHearingRoleDao)
            {
                _userHearingRoleDao = userHearingRoleDao;
            }

            public async Task<Unit> Handle(DeleteUserHearingRoleCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<UserHearingRole>(null, new List<string> { nameof(UserHearingRole.HearingRole) });
                var userHearingRole = await _userHearingRoleDao.GetAsync(request.Id, includes);

                if (userHearingRole == null)
                {
                    throw new NotFoundException(nameof(UserHearingRole), request.Id);
                }

                if (userHearingRole.HearingRole.Role == HearingRole.HEARING_OWNER)
                {
                    throw new Exception("Cannot delete UserHearingRole with Owner role");
                }

                await _userHearingRoleDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}