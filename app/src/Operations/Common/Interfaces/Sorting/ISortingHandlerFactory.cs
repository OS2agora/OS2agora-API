using System;
using System.Collections.Generic;
using System.Text;

namespace Agora.Operations.Common.Interfaces.Sorting
{
    public interface ISortingHandlerFactory
    {
        ISortingHandler<T> GetHandler<T>();
    }
}
