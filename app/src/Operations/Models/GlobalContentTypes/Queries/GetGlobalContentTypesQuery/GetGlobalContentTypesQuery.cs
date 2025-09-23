using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.GlobalContentTypes.Queries.GetGlobalContentTypesQuery
{
    public class GetGlobalContentTypesQuery : IRequest<List<GlobalContentType>>
    {
        public class GetGlobalContentTypesQueryHandler : IRequestHandler<GetGlobalContentTypesQuery, List<GlobalContentType>>
        {
            private readonly IGlobalContentTypeDao _globalContentTypeDao;

            public GetGlobalContentTypesQueryHandler(IGlobalContentTypeDao globalContentTypeDao)
            {
                _globalContentTypeDao = globalContentTypeDao;
            }

            public async Task<List<GlobalContentType>> Handle(GetGlobalContentTypesQuery request,
                CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<GlobalContentType>();
                var globalContentTypes = await _globalContentTypeDao.GetAllAsync(includes);
                return globalContentTypes;
            }
        }
    }
}