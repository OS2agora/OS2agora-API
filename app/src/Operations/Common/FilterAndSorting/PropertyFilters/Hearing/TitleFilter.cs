using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HearingProperties = Agora.Operations.Common.Constants.PropertyFilterAndSorting.HearingProperties;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing
{
    public class TitleFilter : BasePropertyFilter<Agora.Models.Models.Hearing>
    {
        public override string Property => HearingProperties.Title;
        public override string Group => HearingProperties.Groups.Title;

        public override List<string> Includes => HearingProperties.Includes.Title;

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetEqualOperationExpression(string value)
        {
            return hearing =>
                hearing.Contents != null && 
                hearing.Contents.Where(content => content.Field.FieldType.Type == Agora.Models.Enums.FieldType.TITLE)
                    .Any(content =>
                        content.TextContent != null &&
                        string.Equals(content.TextContent, value, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override Expression<Func<Agora.Models.Models.Hearing, bool>> GetContainsOperationExpression(string value)
        {
            return hearing =>
                hearing.Contents != null &&
                hearing.Contents
                    .Where(content => content.Field.FieldType.Type == Agora.Models.Enums.FieldType.TITLE)
                    .Any(content => 
                        content.TextContent != null && 
                        content.TextContent.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
