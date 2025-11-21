using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Agora.Models.Common.CustomResponse;
using Agora.Operations.Common.CustomRequests;
using Agora.Models.Common.CustomResponse.Pagination;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces;

namespace Agora.Operations.Common.Behaviours
{
    public class PaginationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : PaginationRequest<TResponse>
    {
        private readonly IPaginationValidator _validator;

        public PaginationBehaviour(IPaginationValidator validator)
        {
            _validator = validator;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var paginationParameters = request?.PaginationParameters;

            if (paginationParameters == null)
            {
                return await next();
            }

            var paginationParametersAreValid = _validator.ValidatePaginationParameters(paginationParameters);

            bool responseIsEnumerable = typeof(TResponse).GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (!responseIsEnumerable || !paginationParametersAreValid)
            {
                return await next();
            }

            TResponse response = await next();

            return PaginateResponse(response, paginationParameters);
        }

        public TResponse PaginateResponse(TResponse response, PaginationParameters paginationParameters)
        {
            // Since we are now sure about TResponse being a IEnumerable<X> we can dynamic give it to PaginateList
            dynamic changedResult = response;

            return PaginateList(changedResult, paginationParameters);

        }

        private IEnumerable<T> PaginateList<T>(IEnumerable<T> response,
            PaginationParameters paginationParameters)
        {
            var metaData = GetPaginationMetaData(response, paginationParameters);

            var take = metaData.PageSize;
            var skip = (metaData.PageIndex - 1) * take;

            // If pageIndex is greater than totalPages, we just return an empty list
            var paginatedData = metaData.PageIndex > metaData.TotalPages
                ? new List<T>()
                : response.Skip(skip).Take(take);

            return new ResponseList<T>(paginatedData, response, ResponseListMetaDataKeys.Pagination, metaData);

        }

        private PaginationMetaData GetPaginationMetaData<T>(IEnumerable<T> response,
            PaginationParameters paginationParameters)
        {
            var itemsList = response.ToList();
            var pageIndex = (int)paginationParameters.PageIndex;
            var pageSize = (int)paginationParameters.PageSize;
            var totalItems = itemsList.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);

            return new PaginationMetaData
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }
    }
}
