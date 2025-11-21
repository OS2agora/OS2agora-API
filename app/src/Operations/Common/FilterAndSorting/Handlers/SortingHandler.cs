using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Agora.Models.Common;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Interfaces.Sorting;

namespace Agora.Operations.Common.FilterAndSorting.Handlers
{
    public class SortingHandler<T> : ISortingHandler<List<T>> where T : BaseModel
    {
        private readonly IEnumerable<IPropertySorting<T>> _propertySortings;
        private readonly ISortingValidator _validator;

        public SortingHandler(IEnumerable<IPropertySorting<T>> propertySortings, ISortingValidator validator)
        {
            _propertySortings = propertySortings;
            _validator = validator;
        }

        public void ValidateSorting(SortingParameters sortingParameters)
        {
            _validator.ValidateSorting(sortingParameters, _propertySortings);
        }

        public List<T> ApplySorting(List<T> items, SortingParameters sortingParameters)
        {
            var orderByExpression = GetOrderByExpression(sortingParameters?.Sorting);

            if (orderByExpression == null)
            {
                return items.OrderBy(x => x.Id).ToList();
            }

            if (sortingParameters.Sorting.Desc)
            {
                return items.AsQueryable().OrderByDescending(orderByExpression).ThenBy(x => x.Id).ToList();
            }

            return items.AsQueryable().OrderBy(orderByExpression).ThenBy(x => x.Id).ToList();
        }

        public List<string> GetIncludes(SortingParameters sortingParameters)
        {
            var includes = new List<string>();
            var typeSorting = _propertySortings.FirstOrDefault(s => s.Property == sortingParameters.Sorting.Property);
            if (typeSorting != null)
            {
                includes.AddRange(typeSorting.Includes);
            }

            return includes;
        }

        private Expression<Func<T, object>> GetOrderByExpression(Sorting sorting)
        {
            if (sorting == null)
            {
                return null;
            }
            var typeSorting = _propertySortings.FirstOrDefault(s => s.Property == sorting.Property);

            if (typeSorting == null)
            {
                return null;
            }

            return typeSorting.GetSortingExpression();
        }
    }
}
