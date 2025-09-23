using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Extensions;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.SubjectAreas.Command.DeleteSubjectArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class DeleteSubjectAreaCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteSubjectAreaCommandHandler : IRequestHandler<DeleteSubjectAreaCommand>
        {
            private readonly ISubjectAreaDao _subjectAreaDao;

            public DeleteSubjectAreaCommandHandler(ISubjectAreaDao subjectAreaDao)
            {
                _subjectAreaDao = subjectAreaDao;
            }

            public async Task<Unit> Handle(DeleteSubjectAreaCommand request, CancellationToken cancellationToken)
            {
                var includesList = new List<string>() {nameof(SubjectArea.Hearings)};
                var subjectArea = await _subjectAreaDao.GetAsync(request.Id, IncludeProperties.Create<SubjectArea>(null, includesList));

                if (subjectArea == null)
                {
                    throw new NotFoundException(nameof(SubjectArea), request.Id);
                }

                if (subjectArea.Hearings.Count != 0)
                {
                    throw new Exception("Cannot delete SubjectArea that has related Hearings");
                }

                await _subjectAreaDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}