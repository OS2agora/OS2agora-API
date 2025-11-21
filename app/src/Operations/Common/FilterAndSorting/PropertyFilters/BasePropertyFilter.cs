using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces.Filters;

namespace Agora.Operations.Common.FilterAndSorting.PropertyFilters
{
    public abstract class BasePropertyFilter<T> : IPropertyFilter<T>
    {
        public abstract string Property { get; }
        public abstract string Group { get; }
        public abstract List<string> Includes { get; }

        public Expression<Func<T, bool>> GetFilterExpression(string operation, string value)
        {
            switch (operation)
            {
                case var _ when operation == FilterOperations.Equal.Name:
                    return GetEqualOperationExpression(value);
                case var _ when operation == FilterOperations.Contains.Name:
                    return GetContainsOperationExpression(value);
                default:
                    throw new NotSupportedException($"The operation '{operation}' is not supported.");
            }
        }

        protected virtual Expression<Func<T, bool>> GetEqualOperationExpression(string value)
        {
            throw new NotImplementedException();
        }

        protected virtual Expression<Func<T, bool>> GetContainsOperationExpression(string value)
        {
            throw new NotImplementedException();
        }



    }
}
