using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.GlobalContents.Queries.GetLatestGlobalContent
{
    public class GetLatestGlobalContentQuery : IRequest<GlobalContent>
    {
        public int GlobalContentTypeId { get; set; }

        public class GetLatestGlobalContentQueryHandler : IRequestHandler<GetLatestGlobalContentQuery, GlobalContent>
        {
            private readonly IGlobalContentDao _globalContentDao;
            private readonly IGlobalContentTypeDao _globalContentTypeDao;

            public GetLatestGlobalContentQueryHandler(IGlobalContentDao globalContentDao, IGlobalContentTypeDao globalContentTypeDao)
            {
                _globalContentDao = globalContentDao;
                _globalContentTypeDao = globalContentTypeDao;
            }

            public async Task<GlobalContent> Handle(GetLatestGlobalContentQuery request, CancellationToken cancellationToken)
            {
                var globalContentType = await _globalContentTypeDao.GetAsync(request.GlobalContentTypeId);

                if (globalContentType == null)
                {
                    throw new NotFoundException(nameof(GlobalContentType), request.GlobalContentTypeId);
                }

                var includes = IncludeProperties.Create<GlobalContent>();
                var globalContent = await _globalContentDao.GetLatestVersionOfTypeAsync(globalContentType.Type, includes);

                if (globalContent == null)
                {
                    throw new NotFoundException($"Latest version of entity: {nameof(GlobalContent)} with type id: {request.GlobalContentTypeId} was not found");
                }

                return globalContent;
            }
        }
    }
}