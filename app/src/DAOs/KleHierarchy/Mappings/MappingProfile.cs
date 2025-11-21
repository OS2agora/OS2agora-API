using AutoMapper;
using Agora.DAOs.KleHierarchy.DTOs;

namespace Agora.DAOs.KleHierarchy.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KleMainGroupDto, Agora.Models.Models.KleHierarchy>(MemberList.Source)
                .ForMember(x => x.KleHierarchyChildren, options => options.MapFrom(x => x.Groups))
                .ForMember(x => x.Name, options => options.MapFrom(x => x.Title))
                .ReverseMap();

            CreateMap<KleGroupDto, Agora.Models.Models.KleHierarchy>(MemberList.Source)
                .ForMember(x => x.KleHierarchyChildren, options => options.MapFrom(x => x.Topics))
                .ReverseMap();

            CreateMap<KleTopicDto, Agora.Models.Models.KleHierarchy>(MemberList.Source)
                .ReverseMap();
        }
    }
}