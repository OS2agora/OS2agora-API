using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationSources.Queries.GetInvitationSources
{
    [PreAuthorize("HasRole('HearingCreator')")]
    public class GetInvitationSourcesQuery : IRequest<List<InvitationSource>>
    {
        public class GetInvitationSourcesQueryHandler : IRequestHandler<GetInvitationSourcesQuery, List<InvitationSource>>
        {
            private readonly IInvitationSourceDao _invitationSourceDao;

            public GetInvitationSourcesQueryHandler(IInvitationSourceDao invitationSourceDao)
            {
                _invitationSourceDao = invitationSourceDao;
            }

            public async Task<List<InvitationSource>> Handle(GetInvitationSourcesQuery request,
                CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<InvitationSource>();
                var invitationSources = await _invitationSourceDao.GetAllAsync(includes);
                return invitationSources;
            }
        }
    }
}