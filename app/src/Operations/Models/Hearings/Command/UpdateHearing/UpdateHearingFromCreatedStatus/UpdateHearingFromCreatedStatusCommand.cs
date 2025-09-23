using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using HearingStatusEnum = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing.UpdateHearingFromCreatedStatus
{
    public class UpdateHearingFromCreatedStatusCommand : IRequest<Hearing>
    {
        public Hearing Hearing { get; set; }

        public class UpdateHearingFromCreatedStatusCommandHandler : IRequestHandler<UpdateHearingFromCreatedStatusCommand, Hearing>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IHearingStatusDao _hearingStatusDao;

            public UpdateHearingFromCreatedStatusCommandHandler(IHearingDao hearingDao, IHearingStatusDao hearingStatusDao)
            {
                _hearingDao = hearingDao;
                _hearingStatusDao = hearingStatusDao;
            }

            public async Task<Hearing> Handle(UpdateHearingFromCreatedStatusCommand request,
                CancellationToken cancellationToken)
            {
                HearingStatus status = await _hearingStatusDao.GetAsync(request.Hearing.HearingStatusId!.Value);
                if (status.Status != HearingStatusEnum.DRAFT)
                {
                    throw new InvalidOperationException($"HearingStatus with ID \"{status.Id}\" does not match the ID of the {nameof(HearingStatusEnum.DRAFT)} HearingStatus.");
                }
                
                var includes = IncludeProperties.Create<Hearing>();
                return await _hearingDao.UpdateAsync(request.Hearing, includes);
            }
        }
    }
}
