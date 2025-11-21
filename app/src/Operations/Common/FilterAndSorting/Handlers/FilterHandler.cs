using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Agora.Models.Common;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Interfaces.Filters;

namespace Agora.Operations.Common.FilterAndSorting.Handlers
{
    public class FilterHandler<T> : IFilterHandler<List<T>> where T : BaseModel
    {
        private readonly IEnumerable<IPropertyFilter<T>> _propertyFilters;
        private readonly IFilterValidator _validator;

        public FilterHandler(IEnumerable<IPropertyFilter<T>> propertyFilters, IFilterValidator validator)
        {
            _propertyFilters = propertyFilters;
            _validator = validator;
        }

        public void ValidateFilters(FilterParameters filterParameters)
        {
            _validator.ValidateFilters(filterParameters, _propertyFilters);
        }

        public List<T> ApplyFilters(List<T> items, FilterParameters filterParameters)
        {
            var filterExpression = GetFilterExpression(filterParameters.Filters);

            if (filterExpression == null)
            {
                return items;
            }

            return items.AsQueryable().Where(filterExpression).ToList();
        }

        public List<string> GetIncludes(FilterParameters filterParameters)
        {
            var includes = new List<string>();
            foreach (var filter in filterParameters.Filters)
            {
                var typeFilter = _propertyFilters.FirstOrDefault(f => f.Property == filter.Property);
                if (typeFilter != null)
                {
                    includes.AddRange(typeFilter.Includes);
                }
            }

            return includes;
        }

        private Expression<Func<T, bool>> GetFilterExpression(List<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression filterExpression = null;

            Dictionary<string, Expression> expressionGroups = new Dictionary<string, Expression>();

            foreach (var filter in filters)
            {
                var typeFilter = _propertyFilters.FirstOrDefault(f => f.Property == filter.Property);
                if (typeFilter == null)
                {
                    continue;
                }

                var expression = typeFilter.GetFilterExpression(filter.Operation, filter.Value);
                if (expression == null)
                {
                    continue;
                }

                var invokedExpression = Expression.Invoke(expression, parameter);
                var binaryExpression = Expression.MakeBinary(ExpressionType.Equal, invokedExpression, Expression.Constant(true));

                var group = typeFilter.Group;

                if (!expressionGroups.TryAdd(group, binaryExpression))
                {
                    expressionGroups[group] = Expression.OrElse(expressionGroups[group], binaryExpression);
                }
            }

            foreach (var expression in expressionGroups)
            {
                if (expression.Value != null)
                {
                    filterExpression = filterExpression == null ? expression.Value : Expression.AndAlso(filterExpression, expression.Value);
                }
            }

            if (filterExpression == null)
            {
                return null;
            }

            return Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
        }
    }
}
