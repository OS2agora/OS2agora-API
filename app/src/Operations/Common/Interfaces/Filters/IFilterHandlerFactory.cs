using System;
using System.Collections.Generic;
using System.Text;

namespace Agora.Operations.Common.Interfaces.Filters
{
    public interface IFilterHandlerFactory
    {
        IFilterHandler<T> GetHandler<T>();
    }
}
