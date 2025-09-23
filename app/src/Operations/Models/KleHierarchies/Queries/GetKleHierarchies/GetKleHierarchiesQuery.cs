using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.KleHierarchies.Queries.GetKleHierarchies
{
    [PreAuthorize("HasRole('Administrator')")]
    public class GetKleHierarchiesQuery : IRequest<List<KleHierarchy>>
    {
        public class GetKleHierarchiesQueryHandler : IRequestHandler<GetKleHierarchiesQuery, List<KleHierarchy>>
        {
            private readonly IKleHierarchyDao _kleHierarchyDao;

            public GetKleHierarchiesQueryHandler(IKleHierarchyDao kleHierarchyDao)
            {
                _kleHierarchyDao = kleHierarchyDao;
            }

            public async Task<List<KleHierarchy>> Handle(GetKleHierarchiesQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<KleHierarchy>();
                var kleHierarchies = await _kleHierarchyDao.GetAllAsync(includes);
                return kleHierarchies;
            }
        }
    }
}