using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class InvitationSourceDto : BaseDto<InvitationSourceDto.InvitationSourceAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class InvitationSourceAttributeDto : BaseAttributeDto
        {
            private string _name;
            private Enums.InvitationSourceType _invitationSourceType;
            private bool _canDeleteIndividuals;
            private string _cprColumnHeader;
            private string _emailColumnHeader;
            private string _cvrColumnHeader;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public Enums.InvitationSourceType InvitationSourceType
            {
                get => _invitationSourceType;
                set { _invitationSourceType = value; PropertyUpdated(); }
            }

            public bool CanDeleteIndividuals
            {
                get => _canDeleteIndividuals;
                set { _canDeleteIndividuals = value; PropertyUpdated(); }
            }

            public string CprColumnHeader
            {
                get => _cprColumnHeader;
                set { _cprColumnHeader = value; PropertyUpdated(); }
            }

            public string EmailColumnHeader
            {
                get => _emailColumnHeader;
                set { _emailColumnHeader = value; PropertyUpdated(); }
            }

            public string CvrColumnHeader
            {
                get => _cvrColumnHeader;
                set { _cvrColumnHeader = value; PropertyUpdated(); }
            }
        }
    }
}
