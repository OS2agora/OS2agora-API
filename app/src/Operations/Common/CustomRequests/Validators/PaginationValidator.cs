using Agora.Models.Common.CustomResponse.Pagination;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;

namespace Agora.Operations.Common.CustomRequests.Validators
{
    public class PaginationValidator : IPaginationValidator
    {
        public bool ValidatePaginationParameters(PaginationParameters paginationParameters)
        {
            // If no paginationParameters, we will not perform pagination
            if (paginationParameters == null)
            {
                return false;
            }

            var pageIndex = paginationParameters.PageIndex;
            var pageSize = paginationParameters.PageSize;

            // If no pageIndex or pageSize, we will not perform pagination
            if (pageIndex == null && pageSize == null)
            {
                return false;
            }

            // Ensure that both parameters are provided
            if (!pageIndex.HasValue || !pageSize.HasValue)
            {
                throw new PaginationException("Both PageSize and PageIndex must be provided for pagination");
            }

            // Ensure that both values are valid
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new PaginationException("PageIndex and PageSize must be greater than 0");
            }

            return true;

        }
    }
}
