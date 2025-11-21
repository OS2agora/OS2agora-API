using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationGroups.Commands.UpdateInvitationGroup
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateInvitationGroupCommand : IRequest<InvitationGroup>
    {
        public InvitationGroup InvitationGroup { get; set; }

        public class UpdateInvitationGroupCommandHandler : IRequestHandler<UpdateInvitationGroupCommand, InvitationGroup>
        {
            private readonly IInvitationGroupDao _invitationGroupDao;

            public UpdateInvitationGroupCommandHandler(IInvitationGroupDao invitationGroupDao)
            {
                _invitationGroupDao = invitationGroupDao;
            }

            public async Task<InvitationGroup> Handle(UpdateInvitationGroupCommand request,
                CancellationToken cancellationToken)
            {
                await ValidateRequest(request);
                var includes = IncludeProperties.Create<InvitationGroup>();
                var invitationGroup = await _invitationGroupDao.UpdateAsync(request.InvitationGroup, includes);
                return invitationGroup;
            }

            private async Task ValidateRequest(UpdateInvitationGroupCommand request)
            {
                var currentInvitationGroups = await _invitationGroupDao.GetAllAsync();
                var invitationGroupToUpdate =
                    currentInvitationGroups.FirstOrDefault(ig => ig.Id == request.InvitationGroup.Id);
                if (invitationGroupToUpdate == null)
                {
                    throw new NotFoundException(nameof(InvitationGroup), request.InvitationGroup.Id);
                }
                if (currentInvitationGroups.Any(ig => ig.Name == request.InvitationGroup.Name))
                {
                    throw new InvalidOperationException($"An invitation group with the name '{request.InvitationGroup.Name}' already exists.");
                }
            }
        }
    }
}