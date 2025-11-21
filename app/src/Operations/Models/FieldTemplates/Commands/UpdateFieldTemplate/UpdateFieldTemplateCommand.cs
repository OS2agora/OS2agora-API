using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Operations.Models.FieldTemplates.Commands.UpdateFieldTemplate
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateFieldTemplateCommand : IRequest<FieldTemplate>
    {
        public FieldTemplate FieldTemplate { get; set; }

        public class UpdateFieldTemplateCommandHandler : IRequestHandler<UpdateFieldTemplateCommand, FieldTemplate>
        {
            private readonly IFieldTemplateDao _fieldTemplateDao;

            public UpdateFieldTemplateCommandHandler(IFieldTemplateDao fieldTemplateDao)
            {
                _fieldTemplateDao = fieldTemplateDao;
            }

            public async Task<FieldTemplate> Handle(UpdateFieldTemplateCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<FieldTemplate>();
                var fieldTemplate = await _fieldTemplateDao.UpdateAsync(request.FieldTemplate, includes);
                return fieldTemplate;
            }
        }
    }
}