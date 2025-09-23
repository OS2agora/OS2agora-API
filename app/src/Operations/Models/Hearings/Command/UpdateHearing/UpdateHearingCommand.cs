using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing.UpdateHearingFromCreatedStatus;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;
using InvalidOperationException = BallerupKommune.Operations.Common.Exceptions.InvalidOperationException;

namespace BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.Hearing.Id)")]
    public class UpdateHearingCommand : IRequest<Hearing>
    {
        public Hearing Hearing { get; set; }
        public List<string> RequestIncludes { get; set; }

        public class UpdateHearingCommandHandler : IRequestHandler<UpdateHearingCommand, Hearing>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IMediator _mediator;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly IPluginService _pluginService;

            public UpdateHearingCommandHandler(IHearingDao hearingDao, IMediator mediator, IHearingStatusDao hearingStatusDao, IPluginService pluginService)
            {
                _hearingDao = hearingDao;
                _mediator = mediator;
                _hearingStatusDao = hearingStatusDao;
                _pluginService = pluginService;
            }


            public async Task<Hearing> Handle(UpdateHearingCommand request, CancellationToken cancellationToken)
            {
                var includeHearingStatus = new List<string> { nameof(Hearing.HearingStatus) };
                var currentHearing = await _hearingDao.GetAsync(request.Hearing.Id, IncludeProperties.Create<Hearing>(null, includeHearingStatus));

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Hearing.Id);
                }

                var hearingStatus = await _hearingStatusDao.GetAllAsync();
                request.Hearing.HearingStatus = hearingStatus.SingleOrDefault(status => status.Id == request.Hearing.HearingStatusId);

                if (request.Hearing.HearingStatus == null)
                {
                    throw new NotFoundException(nameof(BallerupKommune.Models.Models.HearingStatus), request.Hearing.HearingStatus?.Id);
                }

                request.Hearing = await _pluginService.BeforeHearingUpdate(request.Hearing, currentHearing.HearingStatus.Status);

                Hearing result;

                switch (currentHearing.HearingStatus.Status)
                {
                    case HearingStatus.CREATED:
                        var fromCreatedCommand = new UpdateHearingFromCreatedStatusCommand
                        {
                            Hearing = request.Hearing,
                        };
                        result = await _mediator.Send(fromCreatedCommand, cancellationToken);
                        break;
                    case HearingStatus.DRAFT:
                    case HearingStatus.AWAITING_STARTDATE:
                    case HearingStatus.ACTIVE:
                    case HearingStatus.AWAITING_CONCLUSION:
                        if (request.Hearing.HearingStatus.Id != currentHearing.HearingStatus.Id)
                        {
                            throw new InvalidOperationException($"Invalid hearing status transition: {request.Hearing.HearingStatus.Status} => {currentHearing.HearingStatus.Status}");
                        }
                        result = await _hearingDao.UpdateAsync(request.Hearing);
                        break;
                    case HearingStatus.CONCLUDED:
                        throw new InvalidOperationException("Cannot updated a closed hearing");
                    default:
                        throw new InvalidOperationException("Unable to recognize HearingStatus");
                }

                var includes = IncludeProperties.Create<Hearing>(request.RequestIncludes, null);
                result = await _pluginService.AfterHearingUpdate(result, currentHearing.HearingStatus.Status);
                result = await _hearingDao.UpdateAsync(result, includes);

                await _pluginService.NotifyAfterChangeHearingStatus(currentHearing.Id);

                return result;
            }
        }
    }
}