using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationGroupMappings.Commands.UpdateInvitationGroupMappings
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateInvitationGroupMappingsCommand : IRequest<List<InvitationGroupMapping>>
    {
        public List<InvitationGroupMapping> InvitationGroupMappings { get; set; }
        public int HearingTypeId { get; set; }

        public class UpdateInvitationGroupMappingsCommandHandler : IRequestHandler<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>
        {
            private readonly IInvitationGroupMappingDao _invitationGroupMappingDao;
            private readonly IHearingTypeDao _hearingTypeDao;
            private readonly IInvitationGroupDao _invitationGroupDao;

            public UpdateInvitationGroupMappingsCommandHandler(IInvitationGroupMappingDao invitationGroupMappingDao, IHearingTypeDao hearingTypeDao, IInvitationGroupDao invitationGroupDao)
            {
                _invitationGroupMappingDao = invitationGroupMappingDao;
                _hearingTypeDao = hearingTypeDao;
                _invitationGroupDao = invitationGroupDao;
            }

            public async Task<List<InvitationGroupMapping>> Handle(UpdateInvitationGroupMappingsCommand request, CancellationToken cancellationToken)
            {
                var hearingTypeIncludes = IncludeProperties.Create<HearingType>(null, new List<string> 
                { 
                    nameof(HearingType.InvitationGroupMappings), 
                    $"{nameof(HearingType.InvitationGroupMappings)}.{nameof(InvitationGroupMapping.InvitationGroup)}" 
                });
                var hearingType = await _hearingTypeDao.GetAsync(request.HearingTypeId, hearingTypeIncludes);

                var invitationGroups = await _invitationGroupDao.GetAllAsync();

                if (hearingType == null)
                {
                    throw new NotFoundException(nameof(HearingType), request.HearingTypeId);
                }

                // Remove duplicated invitationGroupMappings entries from the request
                // Filter invitationGroupMappings where InvitationGroupId matches existing entity
                var distinctInvitationGroupMappings = request.InvitationGroupMappings
                    .GroupBy(invitationGroupMapping => new { HearingTypeId = invitationGroupMapping.HearingTypeId, InvitationGroupId = invitationGroupMapping.InvitationGroupId })
                    .Select(group => group.First())
                    .Where(invitationGroupMapping => invitationGroups.Any(invitationGroup => invitationGroup.Id == invitationGroupMapping.InvitationGroupId)).ToList();

                // Find invitationGroupMappings to create
                var invitationGroupMappingsToCreate = distinctInvitationGroupMappings.Where(invitationGroupMapping =>
                    hearingType.InvitationGroupMappings.All(x => x.InvitationGroupId != invitationGroupMapping.InvitationGroupId)).ToList();


                // Find invitationGroupMappings to delete
                var invitationGroupMappingsToDelete = hearingType.InvitationGroupMappings.Where(invitationGroupMapping =>
                    request.InvitationGroupMappings.All(x => x.InvitationGroupId != invitationGroupMapping.InvitationGroupId)).ToList();

                // Delete invitationGroupMappings
                var idsToDelete = invitationGroupMappingsToDelete.Select(invitationGroupMapping => invitationGroupMapping.Id).ToArray();
                await _invitationGroupMappingDao.DeleteRangeAsync(idsToDelete);

                // Create invitationGroupMappings
                var invitationGroupMappingIncludes = IncludeProperties.Create<InvitationGroupMapping>(null, new List<string>
                {
                    nameof(InvitationGroupMapping.HearingType),
                    nameof(InvitationGroupMapping.InvitationGroup)
                });

                var allInvitationGroupMappings = await _invitationGroupMappingDao.CreateRangeAsync(invitationGroupMappingsToCreate, invitationGroupMappingIncludes);
                var allInvitationGroupMappingsForHearingType = allInvitationGroupMappings.Where(x => x.HearingTypeId == request.HearingTypeId);

                return allInvitationGroupMappingsForHearingType.ToList();
            }
        }
    }
}