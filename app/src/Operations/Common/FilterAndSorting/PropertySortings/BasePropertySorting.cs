using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Agora.Operations.Common.Interfaces.Sorting;

namespace Agora.Operations.Common.FilterAndSorting.PropertySortings
{
    public abstract class BasePropertySorting<T> : IPropertySorting<T>
    {
        public abstract string Property { get; }
        public abstract List<string> Includes { get; }

        public virtual Expression<Func<T, object>> GetSortingExpression()
        {
            throw new NotImplementedException();
        }
    }
}
