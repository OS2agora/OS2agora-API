using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class HearingTypeFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.HearingType;

        public override string Group => HearingProperties.Groups.HearingType;

        public override List<string> Includes => HearingProperties.Includes.HearingType;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            return hearing => hearing.HearingType != null && hearing.HearingType.Name.Equals(value);
        }
    }
}
