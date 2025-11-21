using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Operations.Models.HearingTypes.Commands.CreateHearingType
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