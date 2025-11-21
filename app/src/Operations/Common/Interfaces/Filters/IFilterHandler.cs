using System.Collections.Generic;
using Agora.Models.Common.CustomResponse.SortAndFilter;

namespace Agora.Operations.Common.Interfaces.Filters
{
    public interface IFilterHandler<T>
    {
        void ValidateFilters(FilterParameters filterParameters);
        T ApplyFilters(T items, FilterParameters filterParameters);
        List<string> GetIncludes(FilterParameters filterParameters);
    }
}
