using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using UserCapacity = BallerupKommune.Models.Enums.UserCapacity;

namespace BallerupKommune.Operations.Models.Hearings.Command.UploadReviewers
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.Id)")]
    public class UploadReviewersCommand : IRequest<List<UserHearingRole>>
    {
        public int Id { get; set; }
        public List<UserHearingRole> Reviewers { get; set; }

        public class UploadReviewersCommandHandler : IRequestHandler<UploadReviewersCommand, List<UserHearingRole>>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly IHearingRoleDao _hearingRoleDao;
            private readonly IUserDao _userDao;
            private readonly IPluginService _pluginService;

            public UploadReviewersCommandHandler(IHearingDao hearingDao, IUserHearingRoleDao userHearingRoleDao,
                IHearingRoleDao hearingRoleDao, IUserDao userDao, IPluginService pluginService)
            {
                _hearingDao = hearingDao;
                _userHearingRoleDao = userHearingRoleDao;
                _hearingRoleDao = hearingRoleDao;
                _userDao = userDao;
                _pluginService = pluginService;
            }


            public async Task<List<UserHearingRole>> Handle(UploadReviewersCommand request,
                CancellationToken cancellationToken)
            {
                var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}"
                });
                Hearing hearing = await _hearingDao.GetAsync(request.Id, hearingIncludes);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Id);
                }

                await ValidateUserHearingRoles(request.Reviewers);

                var existingReviewerUserHearingRoles = hearing.UserHearingRoles?.Where(userHearingRole =>
                                                           userHearingRole?.HearingRole?.Role ==
                                                           HearingRole.HEARING_REVIEWER) ??
                                                       Enumerable.Empty<UserHearingRole>();

                foreach (var userHearingRole in existingReviewerUserHearingRoles)
                {
                    await _userHearingRoleDao.DeleteAsync(userHearingRole.Id);
                }

                var newUserHearingRoles = new List<UserHearingRole>();
                var userHearingRoleIncludes = IncludeProperties.Create<UserHearingRole>(null, new List<string>
                {
                    nameof(UserHearingRole.User),
                    nameof(UserHearingRole.Hearing),
                    nameof(UserHearingRole.HearingRole)
                });

                foreach (var newUserHearingRole in request.Reviewers)
                {
                    var result = await _userHearingRoleDao.CreateAsync(newUserHearingRole, userHearingRoleIncludes);
                    newUserHearingRoles.Add(result);
                }

                foreach (var userHearingRole in newUserHearingRoles)
                {
                    await _pluginService.NotifyAfterAddedAsReviewer(hearing.Id, userHearingRole.User.Id);
                }

                return newUserHearingRoles;
            }

            private async Task ValidateUserHearingRoles(List<UserHearingRole> userHearingRoles)
            {
                var hearingRoles = await _hearingRoleDao.GetAllAsync();
                var hearingRoleReviewer = hearingRoles.Single(role => role.Role == HearingRole.HEARING_REVIEWER);

                var userIncludes = IncludeProperties.Create<User>(null, new List<string>
                {
                    nameof(User.UserCapacity)
                });

                foreach (var userHearingRole in userHearingRoles)
                {
                    if (userHearingRole.HearingRoleId != hearingRoleReviewer.Id)
                    {
                        throw new InvalidOperationException(
                            $"HearingRole with Id: {userHearingRole.HearingRoleId} does not match the id of the reviewer hearing role: {hearingRoleReviewer.Id}");
                    }

                    var userId = userHearingRole.UserId;
                    var user = await _userDao.GetAsync(userId, userIncludes);

                    if (user == null)
                    {
                        throw new NotFoundException(nameof(User), userId);
                    }

                    if (user?.UserCapacity != null && user.UserCapacity.Capacity != UserCapacity.EMPLOYEE)
                    {
                        throw new InvalidOperationException(
                            $"User with id: {userId} does not have correct UserCapacity: {UserCapacity.EMPLOYEE}");
                    }
                }
            }
        }
    }
}