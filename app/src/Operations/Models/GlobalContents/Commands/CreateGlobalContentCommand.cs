using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.GlobalContents.Commands
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateGlobalContentCommand : IRequest<GlobalContent>
    {
        public GlobalContent GlobalContent { get; set; }
        public int GlobalContentTypeId { get; set; }

        public class CreateGlobalContentCommandHandler : IRequestHandler<CreateGlobalContentCommand, GlobalContent>
        {
            private readonly IGlobalContentDao _globalContentDao;
            private readonly IGlobalContentTypeDao _globalContentTypeDao;

            public CreateGlobalContentCommandHandler(IGlobalContentDao globalContentDao, IGlobalContentTypeDao globalContentTypeDao)
            {
                _globalContentDao = globalContentDao;
                _globalContentTypeDao = globalContentTypeDao;
            }

            public async Task<GlobalContent> Handle(CreateGlobalContentCommand request, CancellationToken cancellationToken)
            {
                var globalContentType = await _globalContentTypeDao.GetAsync(request.GlobalContentTypeId);

                if (globalContentType == null)
                {
                    throw new NotFoundException(nameof(GlobalContentType), request.GlobalContentTypeId);
                }

                var includes = IncludeProperties.Create<GlobalContent>();
                var currentGlobalContent = await _globalContentDao.GetLatestVersionOfTypeAsync(globalContentType.Type);

                var newGlobalContent = new GlobalContent
                {
                    Content = request.GlobalContent.Content,
                    Version = currentGlobalContent.Version + 1,
                    GlobalContentTypeId = globalContentType.Id
                };

                var result = await _globalContentDao.CreateAsync(newGlobalContent, includes);

                return result;
            }
        }
    }
}