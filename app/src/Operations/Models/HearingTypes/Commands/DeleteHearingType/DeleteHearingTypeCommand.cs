using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;
using Agora.Operations.Common.Exceptions;
using HearingType = Agora.Models.Models.HearingType;

namespace Agora.Operations.Models.HearingTypes.Commands.DeleteHearingType
{
    [PreAuthorize("HasRole('Administrator')")]
    public class DeleteHearingTypeCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteHearingTypeCommandHandler : IRequestHandler<DeleteHearingTypeCommand>
        {
            private readonly IHearingTypeDao _hearingTypeDao;

            public DeleteHearingTypeCommandHandler(IHearingTypeDao hearingTypeDao)
            {
                _hearingTypeDao = hearingTypeDao;
            }

            public async Task<Unit> Handle(DeleteHearingTypeCommand request, CancellationToken cancellationToken)
            {
                var includesList = new List<string> { nameof(HearingType.Hearings) };
                var hearingType = await _hearingTypeDao.GetAsync(request.Id, IncludeProperties.Create<HearingType>(null, includesList));

                if (hearingType == null)
                {
                    throw new NotFoundException(nameof(HearingType), request.Id);
                }

                if (hearingType.Hearings.Count != 0)
                {
                    throw new Exception("Cannot delete HearingType that has related Hearings");
                }

                await _hearingTypeDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}