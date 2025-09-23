using AutoMapper;
using BallerupKommune.DAOs.KleHierarchy.DTOs;

namespace BallerupKommune.DAOs.KleHierarchy.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KleMainGroupDto, BallerupKommune.Models.Models.KleHierarchy>(MemberList.Source)
                .ForMember(x => x.KleHierarchyChildren, options => options.MapFrom(x => x.Groups))
                .ForMember(x => x.Name, options => options.MapFrom(x => x.Title))
                .ReverseMap();

            CreateMap<KleGroupDto, BallerupKommune.Models.Models.KleHierarchy>(MemberList.Source)
                .ForMember(x => x.KleHierarchyChildren, options => options.MapFrom(x => x.Topics))
                .ReverseMap();

            CreateMap<KleTopicDto, BallerupKommune.Models.Models.KleHierarchy>(MemberList.Source)
                .ReverseMap();
        }
    }
}