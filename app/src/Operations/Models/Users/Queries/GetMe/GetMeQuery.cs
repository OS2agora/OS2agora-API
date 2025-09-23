using System.Collections.Generic;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Exceptions;
using NovaSec.Attributes;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Models.Users.Queries.GetMe
{
    [PostFilter("true", "UserHearingRoles.Hearing, UserHearingRoles.Hearing.HearingStatus")]
    public class GetMeQuery : IRequest<User>
    {
        public class GetMeQueryHandler : IRequestHandler<GetMeQuery, User>
        {
            private readonly IUserDao _userDao;
            private readonly ICurrentUserService _currentUserService;

            public GetMeQueryHandler(IUserDao userDAo, ICurrentUserService currentUserService)
            {
                _userDao = userDAo;
                _currentUserService = currentUserService;
            }

            public async Task<User> Handle(GetMeQuery request, CancellationToken cancellationToken)
            {
                var userId = _currentUserService.UserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                var includes = IncludeProperties.Create<User>(null, new List<string>
                {
                    nameof(User.UserHearingRoles),
                    $"{nameof(User.UserHearingRoles)}.{nameof(UserHearingRole.Hearing)}",
                    $"{nameof(User.UserHearingRoles)}.{nameof(UserHearingRole.Hearing)}.{nameof(Hearing.HearingStatus)}",
                    nameof(User.Company),
                    $"{nameof(User.Company)}.{nameof(Company.CompanyHearingRoles)}",
                    $"{nameof(User.Company)}.{nameof(Company.CompanyHearingRoles)}.{nameof(CompanyHearingRole.Hearing)}",
                    $"{nameof(User.Company)}.{nameof(Company.CompanyHearingRoles)}.{nameof(CompanyHearingRole.Hearing)}.{nameof(Hearing.HearingStatus)}"
                });
                var user = await _userDao.FindUserByIdentifier(userId, includes);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), userId);
                }

                // We cannot redact hearings in certain status, so we have to do it manually.
                // If we allowed all hearings a user could potentially see his/her own role on a hearing which isn't visible yet.
                // This should be moved to a feature in the NovaSecurity Framework.
                user.UserHearingRoles = user.UserHearingRoles.Where(x =>
                    x.Hearing.HearingStatus.Status != HearingStatus.DRAFT &&
                    x.Hearing.HearingStatus.Status != HearingStatus.CREATED &&
                    x.Hearing.HearingStatus.Status != HearingStatus.NONE).ToList();

                if (user?.Company != null)
                {
                    user.Company.CompanyHearingRoles = user.Company.CompanyHearingRoles.Where(x =>
                        x.Hearing.HearingStatus.Status != HearingStatus.DRAFT &&
                        x.Hearing.HearingStatus.Status != HearingStatus.CREATED &&
                        x.Hearing.HearingStatus.Status != HearingStatus.NONE).ToList();
                }

                return user;
            }
        }
    }
}