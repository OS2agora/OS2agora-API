using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Agora.Operations.Common.Interfaces.Sorting
{
    public interface IPropertySorting<T>
    {
        string Property { get; }
        List<string> Includes { get; }
        Expression<Func<T, object>> GetSortingExpression();
    }
}
