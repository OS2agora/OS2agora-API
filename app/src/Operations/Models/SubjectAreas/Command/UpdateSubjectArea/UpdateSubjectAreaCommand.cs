using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Extensions;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.SubjectAreas.Command.UpdateSubjectArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateSubjectAreaCommand : IRequest<SubjectArea>
    {
        public SubjectArea SubjectArea { get; set; }
        public class UpdateSubjectAreaCommandHandler : IRequestHandler<UpdateSubjectAreaCommand, SubjectArea>
        {
            private readonly ISubjectAreaDao _subjectAreaDao;

            public UpdateSubjectAreaCommandHandler(ISubjectAreaDao subjectAreaDao)
            {
                _subjectAreaDao = subjectAreaDao;
            }

            public async Task<SubjectArea> Handle(UpdateSubjectAreaCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<SubjectArea>();
                var subjectArea = await _subjectAreaDao.UpdateAsync(request.SubjectArea, includes);
                return subjectArea;
            }
        }
    }
}