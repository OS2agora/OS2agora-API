using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HearingStatusEnum = Agora.Models.Enums.HearingStatus;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class HearingStatusFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.HearingStatus;
        public override string Group => HearingProperties.Groups.HearingStatus;

        public override List<string> Includes => HearingProperties.Includes.HearingStatus;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            if (!Enum.TryParse(typeof(HearingStatusEnum), value, out var enumValue))
            {
                return null;
            }

            var hearingStatusValue = (HearingStatusEnum)enumValue;

            return hearing => hearing.HearingStatus.Status == hearingStatusValue;
        }
    }
}
