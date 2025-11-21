using System;
using Agora.Operations.Common.Interfaces.Filters;

namespace Agora.Operations.Common.FilterAndSorting.Factories
{
    public class FilterHandlerFactory : IFilterHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FilterHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFilterHandler<T> GetHandler<T>()
        {
            var itemType = typeof(T);
            var handlerType = typeof(IFilterHandler<>).MakeGenericType(itemType);
            var handler = (IFilterHandler<T>)_serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"Handler for type {typeof(T).Name} not registered.");
            }
            return handler;
        }
    }
}
