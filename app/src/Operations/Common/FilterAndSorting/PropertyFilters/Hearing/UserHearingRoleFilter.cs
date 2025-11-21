using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Agora.Operations.Common.Interfaces;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class UserHearingRoleFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        private ICurrentUserService _currentUserService;

        public UserHearingRoleFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override string Property => HearingProperties.UserHearingRole;
        public override string Group => HearingProperties.Groups.HearingRole;

        public override List<string> Includes => HearingProperties.Includes.UserHearingRole;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            if (!Enum.TryParse(typeof(HearingRoleEnum), value, out var enumValue))
            {
                return null;
            }

            var userId = _currentUserService.DatabaseUserId;

            var hearingRoleValue = (HearingRoleEnum)enumValue;

            return hearing => hearing.UserHearingRoles.Any(role => role.HearingRole.Role == hearingRoleValue && role.UserId == userId);
        }
    }
}
