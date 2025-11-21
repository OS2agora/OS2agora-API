using System.Collections.Generic;
using Agora.Models.Common.CustomResponse.SortAndFilter;

namespace Agora.Operations.Common.Interfaces.Sorting
{
    public interface ISortingHandler<T>
    {
        void ValidateSorting(SortingParameters sortingParameters);
        T ApplySorting(T items, SortingParameters sortingParameters);
        List<string> GetIncludes(SortingParameters sortingParameters);
    }
}
