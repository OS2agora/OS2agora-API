using AutoMapper;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.Models.Models.Esdh;

namespace Agora.DAOs.Esdh.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SagDtoV10, Case>()
                .ForMember(x => x.EsdhTitle, options => options.MapFrom(x => x.Nummer))
                .ForMember(x => x.Guid, options => options.MapFrom(x => x.SagIdentity))
                .ReverseMap();

            CreateMap<SagDto, Case>()
                .ForMember(x => x.EsdhTitle, options => options.MapFrom(x => x.Nummer))
                .ForMember(x => x.Guid, options => options.MapFrom(x => x.SagIdentity))
                .ReverseMap();
        }
    }
}