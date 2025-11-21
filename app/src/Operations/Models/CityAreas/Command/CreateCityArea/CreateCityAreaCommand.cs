using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;

namespace Agora.Operations.Models.CityAreas.Command.CreateCityArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class CreateCityAreaCommand : IRequest<CityArea>
    {
        public CityArea CityArea { get; set; }

        public class CreateCityAreaCommandHandler : IRequestHandler<CreateCityAreaCommand, CityArea>
        {
            private readonly ICityAreaDao _cityAreaDao;

            public CreateCityAreaCommandHandler(ICityAreaDao cityAreaDao)
            {
                _cityAreaDao = cityAreaDao;
            }

            public async Task<CityArea> Handle(CreateCityAreaCommand request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<CityArea>();
                var cityArea = await _cityAreaDao.CreateAsync(request.CityArea, defaultIncludes);
                return cityArea;
            }
        }
    }
}