using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationGroups.Commands.DeleteInvitationGroup
{
    [PreAuthorize("HasRole('Administrator')")]
    public class DeleteInvitationGroupCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteInvitationGroupCommandHandler : IRequestHandler<DeleteInvitationGroupCommand>
        {
            private readonly IInvitationGroupDao _invitationGroupDao;

            public DeleteInvitationGroupCommandHandler(IInvitationGroupDao invitationGroupDao)
            {
                _invitationGroupDao = invitationGroupDao;
            }

            public async Task<Unit> Handle(DeleteInvitationGroupCommand request, CancellationToken cancellationToken)
            {
                var invitationGroup = await _invitationGroupDao.GetAsync(request.Id, IncludeProperties.Create<InvitationGroup>(null, null));

                if (invitationGroup == null)
                {
                    throw new NotFoundException(nameof(InvitationGroup), request.Id);
                }

                await _invitationGroupDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}