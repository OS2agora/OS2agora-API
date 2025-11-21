using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class NotificationContentDto : BaseDto<NotificationContentDto.NotificationContentAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("notificationContentType")
        };

        public class NotificationContentAttributeDto : BaseAttributeDto
        {
            private string _textContent;

            public string TextContent
            {
                get => _textContent;
                set { _textContent = value; PropertyUpdated(); }
            }
        }
    }
}