using System.Linq.Expressions;
using System;
using static Agora.Operations.Common.Constants.PropertyFilterAndSorting;
using System.Collections.Generic;

namespace Agora.Operations.Common.FilterAndSorting.PropertySortings.Hearing
{
    public class DeadlineSorting : BasePropertySorting<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.Deadline;
        public override List<string> Includes => HearingProperties.Includes.Deadline;
        public override Expression<Func<Agora.Models.Models.Hearing, object>> GetSortingExpression()
        {
            return hearing => hearing.Deadline;
        }
    }
}
