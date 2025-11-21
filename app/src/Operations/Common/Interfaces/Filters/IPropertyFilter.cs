using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Agora.Operations.Common.Interfaces.Filters
{
    public interface IPropertyFilter<T>
    {
        string Property { get; }
        string Group { get; }
        List<string> Includes { get; }
        Expression<Func<T, bool>> GetFilterExpression(string operation, string value);
    }
}
