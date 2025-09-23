using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.SubjectAreas.Command.CreateSubjectArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateSubjectAreaCommand : IRequest<SubjectArea>
    {
        public SubjectArea SubjectArea { get; set; }

        public class CreateSubjectAreaCommandHandler : IRequestHandler<CreateSubjectAreaCommand, SubjectArea>
        {
            private readonly ISubjectAreaDao _subjectAreaDao;

            public CreateSubjectAreaCommandHandler(ISubjectAreaDao subjectAreaDao)
            {
                _subjectAreaDao = subjectAreaDao;
            }

            public async Task<SubjectArea> Handle(CreateSubjectAreaCommand request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<SubjectArea>();
                var subjectArea = await _subjectAreaDao.CreateAsync(request.SubjectArea, defaultIncludes);
                return subjectArea;
            }
        }
    }
}