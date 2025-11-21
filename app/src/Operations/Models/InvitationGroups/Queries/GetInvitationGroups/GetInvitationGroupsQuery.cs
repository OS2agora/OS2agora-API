using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationGroups.Queries.GetInvitationGroups
{
    [PreAuthorize("HasAnyRole(['Administrator', 'HearingOwner'])")]
    public class GetInvitationGroupsQuery : IRequest<List<InvitationGroup>>
    {
        public class GetHearingTypesQueryHandler : IRequestHandler<GetInvitationGroupsQuery, List<InvitationGroup>>
        {
            private readonly IInvitationGroupDao _invitationGroupDao;

            public GetHearingTypesQueryHandler(IInvitationGroupDao invitationGroupDao)
            {
                _invitationGroupDao = invitationGroupDao;
            }

            public async Task<List<InvitationGroup>> Handle(GetInvitationGroupsQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<InvitationGroup>();
                var invitationGroups = await _invitationGroupDao.GetAllAsync(includes);
                return invitationGroups;
            }
        }
    }
}