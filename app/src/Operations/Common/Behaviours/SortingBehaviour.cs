using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Agora.Operations.Common.CustomRequests;
using System.Linq;
using Agora.Operations.Common.Interfaces.Sorting;

namespace Agora.Operations.Common.Behaviours
{
    public class SortingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : SortAndFilterRequest<TResponse>
    {
        private readonly ISortingHandlerFactory _sortingHandlerFactory;

        public SortingBehaviour(ISortingHandlerFactory sortingHandlerFactory)
        {
            _sortingHandlerFactory = sortingHandlerFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var sortingParameters = request?.SortingParameters;

            bool responseIsEnumerable = typeof(TResponse).GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (sortingParameters == null || !responseIsEnumerable || !sortingParameters.hasSorting)
            {
                return await next();
            }
            
            var handler = _sortingHandlerFactory.GetHandler<TResponse>();

            handler.ValidateSorting(sortingParameters);

            var sortingIncludes = handler.GetIncludes(sortingParameters);

            request.AppendIncludes(sortingIncludes);

            TResponse response = await next();

            var sortedResponse = handler.ApplySorting(response, sortingParameters);

            return sortedResponse;
        }
    }
}
