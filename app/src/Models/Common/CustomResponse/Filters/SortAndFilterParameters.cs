using System.Collections.Generic;
using System.Linq;

namespace Agora.Models.Common.CustomResponse.SortAndFilter
{
    public class SortAndFilterParameters
    {
        public List<Filter> Filters { get; set; } = null;
        public Sorting Sorting { get; set; } = null;
        public bool hasFilters => Filters != null && Filters.Any();
        public bool hasSorting => Sorting != null;
    }
}
