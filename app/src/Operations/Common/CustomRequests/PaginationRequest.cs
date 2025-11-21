using Agora.Models.Common.CustomResponse.Pagination;

namespace Agora.Operations.Common.CustomRequests
{
    public abstract class PaginationRequest<TResponse> : SortAndFilterRequest<TResponse>
    {
        public PaginationParameters PaginationParameters { get; set; } = null;
    }
}
