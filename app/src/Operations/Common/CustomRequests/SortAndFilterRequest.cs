using MediatR;
using System.Collections.Generic;
using Agora.Models.Common.CustomResponse.SortAndFilter;

namespace Agora.Operations.Common.CustomRequests
{
    public abstract class SortAndFilterRequest<TResponse> : IRequest<TResponse>
    {
        public List<string> SortAndFilterIncludes { get; set; } = new List<string>();
        public SortingParameters SortingParameters { get; set; } = null;
        public FilterParameters FilterParameters { get; set; } = null;

        public void AppendIncludes(List<string> includes)
        {
            SortAndFilterIncludes.AddRange(includes);
        }
    }
}
