using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.FilterAndSorting.PropertyFilters;
using Agora.Operations.Common.Interfaces.Filters;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Operations.Common.CustomRequests.Validators
{
    public class FilterValidator : IFilterValidator
    {
        public void ValidateFilters<T>(FilterParameters filterParameters, IEnumerable<IPropertyFilter<T>> propertyFilters)
        {
            var errors = new Dictionary<string, string[]>();
            var filterErrors = ValidateFilterParameters(filterParameters.Filters, propertyFilters);

            if (filterErrors.Any())
            {
                errors.Add(SortAndFilterExceptionTypes.FilterError, filterErrors.ToArray());
            }

            if (errors.Any())
            {
                throw new SortAndFilterException(errors);
            }
        }

        private List<string> ValidateFilterParameters<T>(List<Filter> filters, IEnumerable<IPropertyFilter<T>> propertyFilters)
        {
            var errors = new List<string>();

            if (filters != null)
            {

                foreach (var filter in filters)
                {
                    var propertyFilter = propertyFilters.FirstOrDefault(f => f.Property == filter.Property);
                    if (propertyFilter == null)
                    {
                        errors.Add($"Filtering {typeof(T).Name} on property {filter.Property} is not supported");
                        continue;
                    }

                    IsOperationSupported(filter.Operation);

                    if (!IsOperationSupportedForProperty((BasePropertyFilter<T>)propertyFilter, filter.Operation))
                    {
                        errors.Add($"The filter operation {filter.Operation} is not supported for property {filter.Property}");
                    }
                }
            }

            return errors;
        }

        private void IsOperationSupported(string operation)
        {
            if (operation == FilterOperations.Equal.Name)
            {
                return;
            }

            if (operation == FilterOperations.Contains.Name)
            {
                return;
            }

            throw new InvalidOperationException($"The filter operation {operation} is not supported");
        }

        private bool IsOperationSupportedForProperty<T>(BasePropertyFilter<T> filter, string operation)
        {
            string methodName = "";
            switch (operation)
            {
                case var _ when operation == FilterOperations.Equal.Name:
                    methodName = FilterOperations.Equal.MethodName;
                    break;
                case var _ when operation == FilterOperations.Contains.Name:
                    methodName = FilterOperations.Contains.MethodName;
                    break;
            };

            var method = filter.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (method == null)
            {
                return false;
            }

            var baseMethod = typeof(BasePropertyFilter<T>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return method.DeclaringType != baseMethod.DeclaringType;
        }
    }
}
