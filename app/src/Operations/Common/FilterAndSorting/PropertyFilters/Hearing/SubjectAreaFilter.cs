using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class SubjectAreaFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.SubjectArea;
        public override string Group => HearingProperties.Groups.SubjectArea;

        public override List<string> Includes => HearingProperties.Includes.SubjectArea;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            return hearing => hearing.SubjectArea != null && hearing.SubjectArea.Name.Equals(value);
        }
    }
}
