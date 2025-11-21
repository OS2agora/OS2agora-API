using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertySortings.Hearing
{
    public class TitleSorting : BasePropertySorting<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.Title;

        public override List<string> Includes => HearingProperties.Includes.Title;

        public override Expression<Func<Agora.Models.Models.Hearing, object>> GetSortingExpression()
        {
            return hearing => hearing.Contents
                .Where(content => content.Field.FieldType.Type == Agora.Models.Enums.FieldType.TITLE)
                .Select(content => content.TextContent)
                .FirstOrDefault();
        }
    }
}
