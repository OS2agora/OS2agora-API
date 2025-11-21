using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;
using Agora.Operations.Common.Exceptions;

namespace Agora.Operations.Models.FieldTemplates.Commands.CreateFieldTemplate
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateFieldTemplateCommand : IRequest<FieldTemplate>
    {
        public FieldTemplate FieldTemplate { get; set; }

        public class CreateFieldTemplateCommandHandler : IRequestHandler<CreateFieldTemplateCommand, FieldTemplate>
        {
            private readonly IFieldTemplateDao _fieldTemplateDao;
            private readonly IHearingTypeDao _hearingTypeDao;
            private readonly IFieldDao _fieldDao;

            public CreateFieldTemplateCommandHandler(IFieldTemplateDao fieldTemplateDao, IHearingTypeDao hearingTypeDao, IFieldDao fieldDao)
            {
                _fieldTemplateDao = fieldTemplateDao;
                _hearingTypeDao = hearingTypeDao;
                _fieldDao = fieldDao;
            }

            public async Task<FieldTemplate> Handle(CreateFieldTemplateCommand request, CancellationToken cancellationToken)
            {
                var currentHearingType = await _hearingTypeDao.GetAsync(request.FieldTemplate.HearingTypeId);
                var currentField = await _fieldDao.GetAsync(request.FieldTemplate.FieldId);

                if (currentHearingType == null)
                {
                    throw new NotFoundException(nameof(HearingType), request.FieldTemplate.HearingTypeId);
                }

                if (currentField == null)
                {
                    throw new NotFoundException(nameof(Field), request.FieldTemplate.FieldId);
                }

                var defaultIncludes = IncludeProperties.Create<FieldTemplate>();
                var fieldTemplate = await _fieldTemplateDao.CreateAsync(request.FieldTemplate, defaultIncludes);
                return fieldTemplate;
            }
        }
    }
}