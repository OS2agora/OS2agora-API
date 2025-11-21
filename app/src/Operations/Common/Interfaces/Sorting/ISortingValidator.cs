using System.Collections.Generic;
using Agora.Models.Common.CustomResponse.SortAndFilter;

namespace Agora.Operations.Common.Interfaces.Sorting
{
    public interface ISortingValidator
    {
        void ValidateSorting<T>(SortingParameters sortingParameters, IEnumerable<IPropertySorting<T>> propertySortings);
    }
}
