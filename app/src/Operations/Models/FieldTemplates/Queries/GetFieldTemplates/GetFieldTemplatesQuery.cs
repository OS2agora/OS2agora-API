using Agora.Models.Extensions;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Operations.Models.FieldTemplates.Queries.GetFieldTemplates
{
    [PreAuthorize("HasRole('Administrator')")]
    public class GetFieldTemplatesQuery : IRequest<List<FieldTemplate>>
    {
        public class GetFieldTemplatesQueryHandler : IRequestHandler<GetFieldTemplatesQuery, List<FieldTemplate>>
        {
            private readonly IFieldTemplateDao _fieldTemplateDao;

            public GetFieldTemplatesQueryHandler(IFieldTemplateDao fieldTemplateDao)
            {
                _fieldTemplateDao = fieldTemplateDao;
            }

            public async Task<List<FieldTemplate>> Handle(GetFieldTemplatesQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<FieldTemplate>();
                var fieldTemplates = await _fieldTemplateDao.GetAllAsync(includes);
                return fieldTemplates;
            }
        }
    }
}