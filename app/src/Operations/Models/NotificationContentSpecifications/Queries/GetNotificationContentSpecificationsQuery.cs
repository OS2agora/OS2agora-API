using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.NotificationContentSpecifications.Queries
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class GetNotificationContentSpecificationsQuery : IRequest<List<NotificationContentSpecification>>
    {
        public int HearingId { get; set; }

        public class GetNotificationContentSpecificationsQueryHandler : IRequestHandler<GetNotificationContentSpecificationsQuery, List<NotificationContentSpecification>>
        {
            private readonly INotificationContentSpecificationDao _notificationContentSpecificationDao;

            public GetNotificationContentSpecificationsQueryHandler(INotificationContentSpecificationDao notificationContentSpecificationDao)
            {
                _notificationContentSpecificationDao = notificationContentSpecificationDao;
            }

            public async Task<List<NotificationContentSpecification>> Handle(GetNotificationContentSpecificationsQuery request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<NotificationContentSpecification>();

                var notificationContentSpecifications = await _notificationContentSpecificationDao.GetAllAsync(defaultIncludes, ncs => ncs.HearingId == request.HearingId);
                return notificationContentSpecifications;
            }
        }
    }
}