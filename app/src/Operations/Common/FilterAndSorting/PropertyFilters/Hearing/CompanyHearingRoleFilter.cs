using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Agora.Operations.Common.Interfaces;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class CompanyHearingRoleFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        private ICurrentUserService _currentUserService;

        public CompanyHearingRoleFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override string Property => HearingProperties.CompanyHearingRole;
        public override string Group => HearingProperties.Groups.HearingRole;

        public override List<string> Includes => HearingProperties.Includes.CompanyHearingRole;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            if (!Enum.TryParse(typeof(HearingRoleEnum), value, out var enumValue))
            {
                return null;
            }

            var companyId = _currentUserService.CompanyId;

            var hearingRoleValue = (HearingRoleEnum)enumValue;

            return hearing => hearing.CompanyHearingRoles.Any(role => role.HearingRole.Role == hearingRoleValue && role.CompanyId == companyId);
        }
    }
}