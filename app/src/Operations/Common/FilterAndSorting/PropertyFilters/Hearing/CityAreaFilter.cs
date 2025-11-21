using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class CityAreaFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.CityArea;
        public override string Group => HearingProperties.Groups.CityArea;

        public override List<string> Includes => HearingProperties.Includes.CityArea;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            return hearing => hearing.CityArea != null && hearing.CityArea.Name.Equals(value);
        }
    }
}