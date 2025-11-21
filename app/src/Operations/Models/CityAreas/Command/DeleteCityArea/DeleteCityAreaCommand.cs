using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.CityAreas.Command.DeleteCityArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class DeleteCityAreaCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteCityAreaCommandHandler : IRequestHandler<DeleteCityAreaCommand>
        {
            private readonly ICityAreaDao _cityAreaDao;

            public DeleteCityAreaCommandHandler(ICityAreaDao cityAreaDao)
            {
                _cityAreaDao = cityAreaDao;
            }

            public async Task<Unit> Handle(DeleteCityAreaCommand request, CancellationToken cancellationToken)
            {
                var includesList = new List<string>() { nameof(CityArea.Hearings) };
                var cityArea = await _cityAreaDao.GetAsync(request.Id, IncludeProperties.Create<CityArea>(null, includesList));

                if (cityArea == null)
                {
                    throw new NotFoundException(nameof(CityArea), request.Id);
                }

                if (cityArea.Hearings.Count != 0)
                {
                    throw new Exception("Cannot delete CityArea that has related Hearings");
                }

                await _cityAreaDao.DeleteAsync(request.Id);
                return Unit.Value;
            }
        }
    }
}