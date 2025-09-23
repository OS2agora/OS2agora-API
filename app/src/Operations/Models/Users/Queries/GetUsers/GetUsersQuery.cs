using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using UserCapacity = BallerupKommune.Models.Enums.UserCapacity;

namespace BallerupKommune.Operations.Models.Users.Queries.GetUsers
{
    [PreAuthorize("HasAnyRole(['Administrator', 'HearingOwner'])")]
    public class GetUsersQuery : IRequest<List<User>>
    {
        public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<User>>
        {
            private readonly IUserDao _userDao;

            public GetUsersQueryHandler(IUserDao userDAo)
            {
                _userDao = userDAo;
            }

            public async Task<List<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<User>();
                var users = await _userDao.GetAllAsync(includes);
                var result = users.Where(u =>
                        u?.UserCapacity?.Capacity == UserCapacity.EMPLOYEE)
                    .ToList();
                return result;
            }
        }
    }
}