using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class UserDto : BaseDto<UserDto.UserAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class UserAttributeDto : BaseAttributeDto
        {
            private string _personalIdentifier;
            private string _name;
            private string _employeeDisplayName;
            private string _email;

            public string PersonalIdentifier
            {
                get => _personalIdentifier;
                set { _personalIdentifier = value; PropertyUpdated(); }
            }

            public string Email
            {
                get => _email;
                set { _email = value; PropertyUpdated(); }
            }

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public string EmployeeDisplayName
            {
                get => _employeeDisplayName;
                set { _employeeDisplayName = value; PropertyUpdated(); }
            }
        }
    }
}