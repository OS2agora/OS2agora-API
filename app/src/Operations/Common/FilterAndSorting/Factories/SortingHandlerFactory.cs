using System;
using Agora.Operations.Common.Interfaces.Sorting;

namespace Agora.Operations.Common.FilterAndSorting.Factories
{
    public class SortingHandlerFactory : ISortingHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SortingHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISortingHandler<T> GetHandler<T>()
        {
            var itemType = typeof(T);
            var handlerType = typeof(ISortingHandler<>).MakeGenericType(itemType);
            var handler = (ISortingHandler<T>)_serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"Handler for type {typeof(T).Name} not registered.");
            }
            return handler;
        }
    }
}
