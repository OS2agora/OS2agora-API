using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Models.NotificationContentSpecifications.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HearingStatusEnum = Agora.Models.Enums.HearingStatus;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Hearings.Command.UpdateHearing.UpdateHearingFromCreatedStatus
{
    public class UpdateHearingFromCreatedStatusCommand : IRequest<Hearing>
    {
        public Hearing Hearing { get; set; }

        public class UpdateHearingFromCreatedStatusCommandHandler : IRequestHandler<UpdateHearingFromCreatedStatusCommand, Hearing>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly ISender _mediator;

            public UpdateHearingFromCreatedStatusCommandHandler(IHearingDao hearingDao, IHearingStatusDao hearingStatusDao, ISender mediator)
            {
                _hearingDao = hearingDao;
                _hearingStatusDao = hearingStatusDao;
                _mediator = mediator;
            }

            public async Task<Hearing> Handle(UpdateHearingFromCreatedStatusCommand request,
                CancellationToken cancellationToken)
            {
                HearingStatus status = await _hearingStatusDao.GetAsync(request.Hearing.HearingStatusId!.Value);
                if (status.Status != HearingStatusEnum.DRAFT)
                {
                    throw new InvalidOperationException($"HearingStatus with ID \"{status.Id}\" does not match the ID of the {nameof(HearingStatusEnum.DRAFT)} HearingStatus.");
                }

                await _mediator.Send(new CreateNotificationContentSpecificationCommand
                {
                    HearingId = request.Hearing.Id,
                    NotificationTypeEnum = NotificationType.INVITED_TO_HEARING
                }, cancellationToken);

                var includes = IncludeProperties.Create<Hearing>();
                return await _hearingDao.UpdateAsync(request.Hearing, includes);

            }
        }
    }
}
