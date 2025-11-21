using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertySortings.Hearing
{
    public class StartDateSorting : BasePropertySorting<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.StartDate;
        public override List<string> Includes => HearingProperties.Includes.StartDate;

        public override Expression<Func<Agora.Models.Models.Hearing, object>> GetSortingExpression()
        {
            return hearing => hearing.StartDate;
        }
    }
}
