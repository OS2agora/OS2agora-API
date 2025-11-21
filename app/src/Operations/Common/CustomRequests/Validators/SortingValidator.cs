using System.Collections.Generic;
using System.Linq;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.Sorting;

namespace Agora.Operations.Common.CustomRequests.Validators
{
    public class SortingValidator : ISortingValidator
    {

        public void ValidateSorting<T>(SortingParameters sortingParameters, IEnumerable<IPropertySorting<T>> propertySortings)
        {
            var errors = new Dictionary<string, string[]>();
            var sortingErrors = ValidateSortingParameters(sortingParameters.Sorting, propertySortings);

            if (sortingErrors.Any())
            {
                errors.Add(SortAndFilterExceptionTypes.SortingError, sortingErrors.ToArray());
            }

            if (errors.Any())
            {
                throw new SortAndFilterException(errors);
            }
        }

        private List<string> ValidateSortingParameters<T>(Sorting sorting, IEnumerable<IPropertySorting<T>> propertySortings)
        {
            var errors = new List<string>();

            if (sorting != null)
            {
                var propertySorting = propertySortings.FirstOrDefault(s => s.Property == sorting.Property);
                if (propertySorting == null)
                {
                    errors.Add($"Sorting {typeof(T).Name} on property {sorting.Property} is not supported");
                }
            }

            return errors;
        }
    }
}
