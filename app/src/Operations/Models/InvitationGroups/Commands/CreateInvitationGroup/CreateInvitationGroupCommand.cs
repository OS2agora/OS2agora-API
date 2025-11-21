using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationGroups.Commands.CreateInvitationGroup
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateInvitationGroupCommand : IRequest<InvitationGroup>
    {
        public InvitationGroup InvitationGroup { get; set; }

        public class CreateInvitationGroupCommandHandler : IRequestHandler<CreateInvitationGroupCommand, InvitationGroup>
        {
            private readonly IInvitationGroupDao _invitationGroupDao;

            public CreateInvitationGroupCommandHandler(IInvitationGroupDao invitationGroupDao)
            {
                _invitationGroupDao = invitationGroupDao;
            }

            public async Task<InvitationGroup> Handle(CreateInvitationGroupCommand request, CancellationToken cancellationToken)
            {
                await ValidateRequest(request);
                var defaultIncludes = IncludeProperties.Create<InvitationGroup>();
                var invitationGroup = await _invitationGroupDao.CreateAsync(request.InvitationGroup, defaultIncludes);
                return invitationGroup;
            }

            private async Task ValidateRequest(CreateInvitationGroupCommand request)
            {
                var currentInvitationGroups = await _invitationGroupDao.GetAllAsync();
                if (currentInvitationGroups.Any(ig => ig.Name == request.InvitationGroup.Name))
                {
                    throw new InvalidOperationException($"An invitation group with the name '{request.InvitationGroup.Name}' already exists.");
                }
            }
        }
    }
}