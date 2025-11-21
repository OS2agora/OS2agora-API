using Agora.Operations.Common.CustomRequests;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Agora.Operations.Common.Interfaces.Filters;

namespace Agora.Operations.Common.Behaviours
{
    public class FilterBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : SortAndFilterRequest<TResponse>
    {
        private readonly IFilterHandlerFactory _filterHandlerFactory;

        public FilterBehaviour(IFilterHandlerFactory filterHandlerFactory)
        {
            _filterHandlerFactory = filterHandlerFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var filterParameters = request?.FilterParameters;

            bool responseIsEnumerable = typeof(TResponse).GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (filterParameters == null || !responseIsEnumerable || !(filterParameters.hasFilters))
            {
                return await next();
            }

            var handler = _filterHandlerFactory.GetHandler<TResponse>();

            handler.ValidateFilters(filterParameters);

            var filterIncludes = handler.GetIncludes(filterParameters);

            request.AppendIncludes(filterIncludes);

            TResponse response = await next();

            var filteredResponse = handler.ApplyFilters(response, filterParameters);

            return filteredResponse;
        }

    }
}
