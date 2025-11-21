using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Operations.Models.HearingTypes.Commands.UpdateHearingType
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateHearingTypeCommand : IRequest<HearingType>
    {
        public HearingType HearingType { get; set; }

        public class UpdateHearingTypeCommandHandler : IRequestHandler<UpdateHearingTypeCommand, HearingType>
        {
            private readonly IHearingTypeDao _hearingTypeDao;

            public UpdateHearingTypeCommandHandler(IHearingTypeDao hearingTypeDao)
            {
                _hearingTypeDao = hearingTypeDao;
            }

            public async Task<HearingType> Handle(UpdateHearingTypeCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<HearingType>();
                var hearingType = await _hearingTypeDao.UpdateAsync(request.HearingType, includes);
                return hearingType;
            }
        }
    }
}