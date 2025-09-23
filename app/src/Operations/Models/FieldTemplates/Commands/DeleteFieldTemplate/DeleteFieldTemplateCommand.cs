using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.FieldTemplates.Commands.DeleteFieldTemplate
{
    [PreAuthorize("HasRole('Administrator')")]
    public class DeleteFieldTemplateCommand : IRequest
    {
        public int Id { get; set; }
        public int HearingTypeId { get; set; }

        public class DeleteFieldTemplateCommandHandler : IRequestHandler<DeleteFieldTemplateCommand>
        {
            private readonly IFieldTemplateDao _fieldTemplateDao;
            private readonly IHearingTypeDao _hearingTypeDao;

            public DeleteFieldTemplateCommandHandler(IFieldTemplateDao fieldTemplateDao, IHearingTypeDao hearingTypeDao)
            {
                _fieldTemplateDao = fieldTemplateDao;
                _hearingTypeDao = hearingTypeDao;
            }

            public async Task<Unit> Handle(DeleteFieldTemplateCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<HearingType>(null, new List<string> { nameof(HearingType.FieldTemplates) });
                var allHearingTypes = await _hearingTypeDao.GetAllAsync(includes);

                var currentHearingType = allHearingTypes.SingleOrDefault(x =>
                    x.FieldTemplates.Any(y => y.Id == request.Id) && x.Id == request.HearingTypeId);

                if (currentHearingType == null)
                {
                    throw new Exception("FieldTemplate does not exist, or does not exist on the specified HearingType");
                }

                await _fieldTemplateDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}