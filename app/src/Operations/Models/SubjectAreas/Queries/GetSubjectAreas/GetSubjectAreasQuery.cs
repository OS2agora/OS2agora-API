using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;

namespace Agora.Operations.Models.SubjectAreas.Queries.GetSubjectAreas
{
    public class GetSubjectAreasQuery : IRequest<List<SubjectArea>>
    {
        public class GetSubjectAreasQueryHandler : IRequestHandler<GetSubjectAreasQuery, List<SubjectArea>>
        {
            private readonly ISubjectAreaDao _subjectAreaDao;

            public GetSubjectAreasQueryHandler(ISubjectAreaDao subjectAreaDao)
            {
                _subjectAreaDao = subjectAreaDao;
            }

            public async Task<List<SubjectArea>> Handle(GetSubjectAreasQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<SubjectArea>();
                var subjectAreas = await _subjectAreaDao.GetAllAsync(includes);
                return subjectAreas;
            }
        }
    }
}