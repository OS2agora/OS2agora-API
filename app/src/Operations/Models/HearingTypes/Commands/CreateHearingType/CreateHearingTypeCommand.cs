using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.HearingTypes.Commands.CreateHearingType
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateHearingTypeCommand : IRequest<HearingType>
    {
        public HearingType HearingType { get; set; }

        public class CreateHearingTypeCommandHandler : IRequestHandler<CreateHearingTypeCommand, HearingType>
        {
            private readonly IHearingTypeDao _hearingTypeDao;

            public CreateHearingTypeCommandHandler(IHearingTypeDao hearingTypeDao)
            {
                _hearingTypeDao = hearingTypeDao;
            }

            public async Task<HearingType> Handle(CreateHearingTypeCommand request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<HearingType>();
                var hearingType = await _hearingTypeDao.CreateAsync(request.HearingType, defaultIncludes);
                return hearingType;
            }
        }
    }
}