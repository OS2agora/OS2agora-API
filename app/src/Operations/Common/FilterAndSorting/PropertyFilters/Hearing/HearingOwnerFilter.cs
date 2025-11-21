using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class HearingOwnerFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.HearingOwner;
        public override string Group => HearingProperties.Groups.HearingOwner;

        public override List<string> Includes => HearingProperties.Includes.UserHearingRole;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            return hearing => hearing.UserHearingRoles.Any(role => role.HearingRole.Role == HearingRoleEnum.HEARING_OWNER && role.UserId == int.Parse(value));
        }
    }
}