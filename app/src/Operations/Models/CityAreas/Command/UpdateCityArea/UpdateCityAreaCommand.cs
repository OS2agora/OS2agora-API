using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.CityAreas.Command.UpdateCityArea
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateCityAreaCommand : IRequest<CityArea>
    {
        public CityArea CityArea { get; set; }
        public class UpdateCityAreaCommandHandler : IRequestHandler<UpdateCityAreaCommand, CityArea>
        {
            private readonly ICityAreaDao _cityAreaDao;

            public UpdateCityAreaCommandHandler(ICityAreaDao cityAreaDao)
            {
                _cityAreaDao = cityAreaDao;
            }

            public async Task<CityArea> Handle(UpdateCityAreaCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<CityArea>();
                var cityArea = await _cityAreaDao.UpdateAsync(request.CityArea, includes);
                return cityArea;
            }
        }
    }
}