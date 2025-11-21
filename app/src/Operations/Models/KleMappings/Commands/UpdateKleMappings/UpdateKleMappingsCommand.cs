using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Interfaces.Plugins;

namespace Agora.Operations.Models.KleMappings.Commands.UpdateKleMappings
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateKleMappingsCommand : IRequest<List<KleMapping>>
    {
        public List<KleMapping> KleMappings { get; set; }
        public int HearingTypeId { get; set; }

        public class UpdateKleMappingsCommandHandler : IRequestHandler<UpdateKleMappingsCommand, List<KleMapping>>
        {
            private readonly IKleMappingDao _kleMappingDao;
            private readonly IHearingTypeDao _hearingTypeDao;
            private readonly IKleHierarchyDao _kleHierarchyDao;
            private readonly IPluginService _pluginService;


            public UpdateKleMappingsCommandHandler(IKleHierarchyDao kleHierarchyDao, IHearingTypeDao hearingTypeDao, IKleMappingDao kleMappingDao, IPluginService pluginService)
            {
                _kleHierarchyDao = kleHierarchyDao;
                _hearingTypeDao = hearingTypeDao;
                _kleMappingDao = kleMappingDao;
                _pluginService = pluginService;
            }

            public async Task<List<KleMapping>> Handle(UpdateKleMappingsCommand request, CancellationToken cancellationToken)
            {
                var hearingTypeIncludes = IncludeProperties.Create<HearingType>(null, new List<string>
                {
                    nameof(HearingType.KleMappings),
                    $"{nameof(HearingType.KleMappings)}.{nameof(KleMapping.KleHierarchy)}"
                });
                var hearingType = await _hearingTypeDao.GetAsync(request.HearingTypeId, hearingTypeIncludes);

                var kleHierarchies = await _kleHierarchyDao.GetAllAsync();

                if (hearingType == null)
                {
                    throw new NotFoundException(nameof(HearingType), request.HearingTypeId);
                }

                // Remove duplicated kleMapping entries from the request
                // Filter kleMappings where kleHierarchyId matches existing entity
                var distinctKleMappings = request.KleMappings
                    .GroupBy(kleMapping => new { HearingTypeId = kleMapping.HearingTypeId, KleHierarchyId = kleMapping.KleHierarchyId })
                    .Select(group => group.First())
                    .Where(kleMapping => kleHierarchies.Any(kleHierarchy => kleHierarchy.Id == kleMapping.KleHierarchyId)).ToList();

                // Find kleMappings to create
                var kleMappingsToCreate = distinctKleMappings.Where(kleMapping =>
                    hearingType.KleMappings.All(x => x.KleHierarchyId != kleMapping.KleHierarchyId)).ToList();

                // Validate kleMappings to create
                await _pluginService.ValidateKleMapping(kleMappingsToCreate);

                // Find kleMappings to delete
                var kleMappingsToDelete = hearingType.KleMappings.Where(kleMapping =>
                    request.KleMappings.All(x => x.KleHierarchyId != kleMapping.KleHierarchyId)).ToList();

                // Delete kleMappings
                var idsToDelete = kleMappingsToDelete.Select(kleMapping => kleMapping.Id).ToArray();
                await _kleMappingDao.DeleteRangeAsync(idsToDelete);

                // Create kleMappings
                var kleMappingIncludes = IncludeProperties.Create<KleMapping>(null, new List<string>
                {
                    nameof(KleMapping.HearingType),
                    nameof(KleMapping.KleHierarchy)
                });

                var allKleMappings = await _kleMappingDao.CreateRangeAsync(kleMappingsToCreate, kleMappingIncludes);
                var allKleMappingsForHearingType = allKleMappings.Where(x => x.HearingTypeId == request.HearingTypeId);

                return allKleMappingsForHearingType.ToList();
            }
        }
    }
}