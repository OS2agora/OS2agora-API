using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class NotificationContentSpecificationDto : BaseDto<NotificationContentSpecificationDto.NotificationContentSpecificationAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class NotificationContentSpecificationAttributeDto : BaseAttributeDto
        {
        }
    }
}