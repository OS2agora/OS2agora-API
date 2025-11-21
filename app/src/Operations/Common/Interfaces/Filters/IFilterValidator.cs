using System.Collections.Generic;
using Agora.Models.Common.CustomResponse.SortAndFilter;

namespace Agora.Operations.Common.Interfaces.Filters
{
    public interface IFilterValidator
    {
        void ValidateFilters<T>(FilterParameters filterParameters, IEnumerable<IPropertyFilter<T>> propertyFilters);
    }
}
