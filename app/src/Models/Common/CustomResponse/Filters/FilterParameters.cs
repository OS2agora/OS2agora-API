using System.Collections.Generic;
using System.Linq;

namespace Agora.Models.Common.CustomResponse.SortAndFilter
{
    public class FilterParameters
    {
        public List<Filter> Filters { get; set; } = null;
        public bool hasFilters => Filters != null && Filters.Any();
    }
}
