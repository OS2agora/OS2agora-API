using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.CityAreas.Queries.GetCityAreas
{
    public class GetCityAreasQuery : IRequest<List<CityArea>>
    {
        public class GetCityAreasQueryHandler : IRequestHandler<GetCityAreasQuery, List<CityArea>>
        {
            private readonly ICityAreaDao _cityAreaDao;

            public GetCityAreasQueryHandler(ICityAreaDao cityAreaDao)
            {
                _cityAreaDao = cityAreaDao;
            }

            public async Task<List<CityArea>> Handle(GetCityAreasQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<CityArea>();
                var cityAreas = await _cityAreaDao.GetAllAsync(includes);
                return cityAreas;
            }
        }
    }
}