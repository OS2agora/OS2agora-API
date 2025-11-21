using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;

namespace Agora.Operations.Models.HearingRoles.Queries.GetHearingRoles
{
    public class GetHearingRolesQuery : IRequest<List<HearingRole>>
    {
        public class GetHearingRolesQueryHandler : IRequestHandler<GetHearingRolesQuery, List<HearingRole>>
        {
            private readonly IHearingRoleDao _hearingRoleDao;

            public GetHearingRolesQueryHandler(IHearingRoleDao hearingRoleDao)
            {
                _hearingRoleDao = hearingRoleDao;
            }

            public async Task<List<HearingRole>> Handle(GetHearingRolesQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<HearingRole>();
                var hearingRoles = await _hearingRoleDao.GetAllAsync(includes);
                return hearingRoles;
            }
        }
    }
}